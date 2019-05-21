using Debug = IW.Logger;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BfgAnalytics;
using UnityEngine;

public class FogsDataManager : IECSComponent, IDataManager, IDataLoader<FogsDataManager>
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;

    private ECSEntity context;
    
    public void OnRegisterEntity(ECSEntity entity)
    {
        context = entity;
        
        Reload();
    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {
        context = null;
    }
    
    public List<FogDef> Fogs { get; set; }
    
    public Dictionary<BoardPosition, FogDef> VisibleFogPositions;
    public Dictionary<BoardPosition, FogDef> ClearedFogPositions;
    
    public Dictionary<BoardPosition, FogObserver> FogObservers;

    private FogDef lastOpenFog;
    public FogDef LastOpenFog
    {
        get { return lastOpenFog; }
        set { lastOpenFog = value; }
    }
    
    public void Reload()
    {
        Fogs = null;
        VisibleFogPositions = null;
        ClearedFogPositions = null;
        FogObservers = new Dictionary<BoardPosition, FogObserver>();
        LoadData(new HybridConfigDataMapper<FogsDataManager>("configs/fogs.data", NSConfigsSettings.Instance.IsUseEncryption));
    }
    
    public void LoadData(IDataMapper<FogsDataManager> dataMapper)
    {
        dataMapper.LoadData((data, error)=> 
        {
            VisibleFogPositions = new Dictionary<BoardPosition, FogDef>();
            ClearedFogPositions = new Dictionary<BoardPosition, FogDef>();
            
            if (string.IsNullOrEmpty(error))
            {
                Fogs = data.Fogs;

                var save = ((GameDataManager)context)?.UserProfile?.GetComponent<FogSaveComponent>(FogSaveComponent.ComponentGuid);
                var completeFogIds = save?.CompleteFogIds ?? new List<string>();
                
                string lastCompleteFogUid = "";
                if (completeFogIds.Count > 0)
                {
                    lastCompleteFogUid = completeFogIds[completeFogIds.Count - 1];
                }
                
                foreach (var def in data.Fogs)
                {
                    var pos = def.GetCenter();
                    
                    if (completeFogIds.Contains(def.Uid))
                    {
                        ClearedFogPositions.Add(pos, def);
                    }
                    else
                    {
                        VisibleFogPositions.Add(pos, def);
                    }

                    if (def.Uid == lastCompleteFogUid)
                    {
                        LastOpenFog = def;
                    }
                }
            }
            else
            {
                Debug.LogWarningFormat("[{0}]: config not loaded", GetType());
            }
        });
    }

    public FogDef GetDef(BoardPosition key)
    {
        return VisibleFogPositions.TryGetValue(key, out var def) == false ? null : def;
    }

    public void RemoveFog(BoardPosition key)
    {
        Debug.Log($"[FogsDataManager] => RemoveFog({key}");

        var def = GetDef(key);
        
        if(VisibleFogPositions.ContainsKey(key) == false) return;
        LastOpenFog = def;
        ClearedFogPositions.Add(key, def);
        VisibleFogPositions.Remove(key);
        
        Analytics.SendFogClearedEvent(def.Uid);

        GameDataService.Current.QuestsManager.StartNewQuestsIfAny();
    }

    public BoardPosition? GetFogPositionByUid(string uid)
    {
        foreach (var pair in VisibleFogPositions)
        {
            if (pair.Value.Uid == uid)
            {              
                return pair.Value.GetCenter();
            }
        }

        return null;
    }

    public FogObserver GetFogObserver(BoardPosition pos)
    {
        return FogObservers.TryGetValue(pos, out var ret) ? ret : null;
    }

    public void UpdateFogObserver(int id)
    {
        foreach (var observer in FogObservers.Values)
        {
            if(observer.Def.HeroId != id) continue;
            
            observer.UpdateResource(0);
        }
    }
    
    public bool IsFogCleared(string uid)
    {
        var pos = GetFogPositionByUid(uid);
        return !pos.HasValue;
    }

    public FogDef GetNextRandomFog()
    {
        List<FogObserver> visibleFogs = VisibleFogPositions.Values.Select(e => GetFogObserver(e.GetCenter())).ToList();
        if (visibleFogs.Count == 0)
        {
            Debug.LogError("[FogsDataManager] => GetNextRandomFog: No visible fog found!");
            return null;
        }

        List<FogObserver> sortedFogs;
        bool ignorePrice = false;
        bool ignoreLevel = false;
        
        List<FogObserver> fogsWithBubbles = visibleFogs.Where(e => e.CanBeReached() && e.RequiredConditionReached()).ToList();

        if (fogsWithBubbles.Count > 0)
        {
            ignoreLevel = true;
            
            sortedFogs = fogsWithBubbles.OrderBy(e => e.Def.Condition.Amount).ToList();
        }
        else
        {
            ignorePrice = true;
            
            sortedFogs = visibleFogs//.OrderBy(e => e.Def.Level)
                                    .OrderByDescending(e => e.CanBeReached())
                                    // .ThenByDescending(e => e.RequiredLevelReached())
                                    // .ThenBy(e => e.Def.Condition.Amount)
                                    .ThenBy(e => e.Def.Level)
                                    .ToList();
        }

        var firstFog = sortedFogs[0];

        List<FogObserver> selectedFogs = new List<FogObserver>
        {
            firstFog
        };

        for (var i = 1; i < sortedFogs.Count; i++)
        {
            var fog = sortedFogs[i];
            if ((fog.CanBeReached() == firstFog.CanBeReached())
             && (fog.RequiredConditionReached() == firstFog.RequiredConditionReached())
             && (fog.Def.Condition.Amount == firstFog.Def.Condition.Amount || ignorePrice)
             && (fog.Def.Level == firstFog.Def.Level || ignoreLevel))
            {
                selectedFogs.Add(fog);
            }
            else
            {
                break;
            }
        }

        var index = Random.Range(0, selectedFogs.Count);
        var ret = selectedFogs[index];
        
#if DEBUG
        var sb = new StringBuilder("[FogsDataManager] => GetNextRandomFog:\n");
        
        foreach (var fog in sortedFogs)
        {
            string tag = ret.Def.Uid == fog.Def.Uid 
                ? "[SELECTED]" 
                : selectedFogs.Any(e => e.Def.Uid == fog.Def.Uid) 
                    ? "[CANDIDATE]" 
                    : "";
            
            sb.AppendLine($"Fog [{fog.Def.Uid}]: Path found: {fog.CanBeReached()}, Required level reached: {fog.RequiredConditionReached()}, Price: {fog.Def.Condition.Amount}, Required level: {fog.Def.Level} {tag}");
        }
        
        Debug.Log(sb);
#endif
        
        return ret.Def;
    }

    public List<GridMeshArea> GetFoggedAreas()
    {
        List<GridMeshArea> ret = new List<GridMeshArea>();

        // INCLUDE
        foreach (var pair in VisibleFogPositions)
        {
            var fogDef = pair.Value;

            if (FogObservers.TryGetValue(pair.Key, out var observer))
            {
                if (!observer.IsActive)
                {
                    continue; 
                }
            }
            else
            {
                continue;
            }
            
            var positions = fogDef.Positions;

            var area = GetFogAreaForPositions(positions, false);

            ret.Add(area);
        }

        // EXCLUDE
        foreach (var pair in ClearedFogPositions)
        {
            var fogDef = pair.Value;
            var positions = fogDef.Positions;

            var area = GetFogAreaForPositions(positions, true);

            ret.Add(area);
        }

        var lockedArea = GetFogAreaForPositions(GameDataService.Current.FieldManager.LockedCells.ToList(), true);
        ret.Add(lockedArea);
        
        //Starting field
        List<BoardPosition> startPositions = new List<BoardPosition>();
        for (int x = 18; x <= 21; x++)
        {
            for (int y = 11; y <= 14; y++)
            {
                startPositions.Add(new BoardPosition(x, y));
            }
        }
        
        startPositions.Add(new BoardPosition(22, 13));
        startPositions.Add(new BoardPosition(22, 14));

        ret.Add(GetFogAreaForPositions(startPositions, true));
        
        return ret;
    }

    /// <summary>
    /// reate GridMeshArea using any list of BoardPosition, even when no real fog there
    /// </summary>
    private static GridMeshArea GetFogAreaForPositions(List<BoardPosition> positions, bool exclude)
    {
        BoardPosition topLeft;
        BoardPosition topRight;
        BoardPosition bottomRight;
        BoardPosition bottomLeft;
        BoardPosition.GetAABB(positions, out topLeft, out topRight, out bottomRight, out bottomLeft);

        int areaW = topRight.X - topLeft.X + 1;
        int areaH = topLeft.Y - bottomLeft.Y + 1;

        GridMeshArea area = new GridMeshArea
        {
            X = bottomLeft.X,
            Y = bottomLeft.Y,
            Matrix = new int[areaW, areaH],
            Exclude = exclude
        };

        foreach (var position in positions)
        {
            int x = position.X - area.X;
            int y = position.Y - area.Y;
            area.Matrix[x, y] = 1;
        }

        return area;
    }

    public void RegisterFogObserver(FogObserver observer)
    {
        FogObservers.Add(observer.Key, observer);
    }
    
    public void UnregisterFogObserver(FogObserver observer)
    {
        FogObservers.Remove(observer.Key);
    }

    public bool SetMana(Piece piece, BoardPosition targetPosition)
    {
        var def = GameDataService.Current.PiecesManager.GetPieceDef(piece.PieceType);
        var target = piece.Context.BoardLogic.GetPieceAt(targetPosition);

        if (def?.SpawnResources == null || def.SpawnResources.Currency != Currency.Mana.Name || target.PieceType != PieceType.Fog.Id) return false;
        
        var observer = target.GetComponent<FogObserver>(FogObserver.ComponentGuid);
        
        if (observer == null || observer.IsRemoved || observer.CanBeFilled(piece, false) == false || observer.CanBeCleared() == false) return false;
        
        observer.Filling(def.SpawnResources.Amount, out var balance);
        observer.OnProgress(targetPosition);
        observer.TempMana.Remove(piece);
        
        if (balance <= 0)
        {
            piece.Context.ActionExecutor.AddAction(new CollapsePieceToAction
            {
                To = targetPosition,
                Positions = new List<BoardPosition> {piece.CachedPosition}
            });
            
            return true;
        }

        var pieces = CurrencyHelper.CurrencyToResourcePieces(balance, Currency.Mana.Name);
        
        const int max = 2;
        var current = 0;
        var result = new Dictionary<int, int>();

        foreach (var pair in pieces)
        {
            if (current == max) break;
            
            var value = Mathf.Clamp(pair.Value, 0, max - current);
            
            current += value;
            result.Add(pair.Key, value);
        }

        var position = observer.Def.GetCenter();
        position.Z = BoardLayer.Piece.Layer;
        
        piece.Context.ActionExecutor.AddAction(new ManaCangeAction
        {
            Target = targetPosition,
            Old = piece.CachedPosition,
            From = position,
            Pieces = result,
        });
        
        return true;
    }
}