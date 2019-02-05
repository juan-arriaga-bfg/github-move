using System.Collections.Generic;
using System.Linq;
using System.Text;
using BfgAnalytics;
using UnityEngine;

public class FogsDataManager : IECSComponent, IDataManager, IDataLoader<FogsDataManager>
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;

    public void OnRegisterEntity(ECSEntity entity)
    {
        Reload();
    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }
    
    public List<FogDef> Fogs { get; set; }
    
    public Dictionary<BoardPosition, FogDef> VisibleFogPositions;
    public Dictionary<BoardPosition, FogDef> ClearedFogPositions;
    
    public Dictionary<BoardPosition, FogObserver> FogObservers;
    private List<FogDef> ActiveFogs;

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
        ActiveFogs   = new List<FogDef>();
        LoadData(new ResourceConfigDataMapper<FogsDataManager>("configs/fogs.data", NSConfigsSettings.Instance.IsUseEncryption));
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

                var save = ProfileService.Current.GetComponent<FogSaveComponent>(FogSaveComponent.ComponentGuid);
                var completeFogPositions = save?.CompleteFogPositions ?? new List<BoardPosition>();
                
                foreach (var def in data.Fogs)
                {
                    var pos = def.GetCenter();
                    
                    if (completeFogPositions.Contains(pos))
                    {
                        ClearedFogPositions.Add(pos, def);
                    }
                    else
                    {
                        VisibleFogPositions.Add(pos, def);
                    }
                }
                
                if (completeFogPositions.Count > 0)
                    LastOpenFog = data.Fogs.Find(fog => fog.GetCenter().Equals(completeFogPositions[completeFogPositions.Count - 1]));
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
        
        List<FogObserver> fogsWithBubbles = visibleFogs.Where(e => e.CanBeReached() && e.RequiredLevelReached()).ToList();

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
             && (fog.RequiredLevelReached() == firstFog.RequiredLevelReached())
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
            
            sb.AppendLine($"Fog [{fog.Def.Uid}]: Path found: {fog.CanBeReached()}, Required level reached: {fog.RequiredLevelReached()}, Price: {fog.Def.Condition.Amount}, Required level: {fog.Def.Level} {tag}");
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
        
        //Starting field
        List<BoardPosition> startPositions = new List<BoardPosition>();
        for (int x = 18; x <= 22; x++)
        {
            for (int y = 11; y <= 14; y++)
            {
                startPositions.Add(new BoardPosition(x, y));
            }
        }
        
        // startPositions.Add(new BoardPosition(19, 9));
        // startPositions.Add(new BoardPosition(20, 9));
        // startPositions.Add(new BoardPosition(21, 9));

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
        if (observer.IsActive)
        {
            ActiveFogs.Add(observer.Def);
            ActiveFogs.Sort((def1, def2) => def1.Level - def2.Level);
        }
    }
    
    public void UnregisterFogObserver(FogObserver observer)
    {
        FogObservers.Remove(observer.Key);
        ActiveFogs.Remove(observer.Def);
    }

    public bool SetMana(Piece piece, BoardPosition targetPosition)
    {
        var def = GameDataService.Current.PiecesManager.GetPieceDef(piece.PieceType);
        var target = piece.Context.BoardLogic.GetPieceAt(targetPosition);

        if (def?.SpawnResources == null || def.SpawnResources.Currency != Currency.Mana.Name || target.PieceType != PieceType.Fog.Id) return false;
        
        var observer = target.GetComponent<FogObserver>(FogObserver.ComponentGuid);
        
        if (observer == null || observer.IsRemoved || observer.CanBeFilled() == false || observer.CanBeCleared() == false) return false;
        
        observer.Filling(def.SpawnResources.Amount);
        
        return true;
    }
}