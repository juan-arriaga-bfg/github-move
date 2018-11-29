using System.Collections.Generic;
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
    
    public List<ItemWeight> DefaultPieceWeights { get; set; }
    public List<FogDef> Fogs { get; set; }
    
    public Dictionary<BoardPosition, FogDef> VisibleFogPositions;
    public Dictionary<BoardPosition, FogDef> ClearedFogPositions;
    
    private Dictionary<BoardPosition, FogObserver> FogObservers;
    private List<FogDef> ActiveFogs;
    
    public void Reload()
    {
        DefaultPieceWeights = null;
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
                DefaultPieceWeights = data.DefaultPieceWeights;
                Fogs = data.Fogs;
                
                var save = ProfileService.Current.GetComponent<FieldDefComponent>(FieldDefComponent.ComponentGuid);

                List<BoardPosition> completeFogPositions = save?.CompleteFogPositions ?? new List<BoardPosition>();
                
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
            }
            else
            {
                Debug.LogWarningFormat("[{0}]: config not loaded", GetType());
            }
        });
    }

    public FogDef GetDef(BoardPosition key)
    {
        FogDef def;
        return VisibleFogPositions.TryGetValue(key, out def) == false ? null : def;
    }

    public void RemoveFog(BoardPosition key)
    {
        Debug.Log($"[FogsDataManager] => RemoveFog({key}");
        
        if(VisibleFogPositions.ContainsKey(key) == false) return;
        
        ClearedFogPositions.Add(key, GetDef(key));
        VisibleFogPositions.Remove(key);

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

    public bool IsFogCleared(string uid)
    {
        var pos = GetFogPositionByUid(uid);
        return !pos.HasValue;
    }

    public FogDef GetRandomActiveFogWithLeastLevel()
    {
        if (ActiveFogs.Count == 0)
        {
            Debug.LogError("[FogsDataManager] => GetUidOfFirstNotClearedFog: No defs found!");
            return null;
        }

        int firstLevel = ActiveFogs[0].Level;
        int lastIndex = -1;
        
        for (int i = 1; i < ActiveFogs.Count; i++)
        {
            var def = ActiveFogs[i];
            if (def.Level == firstLevel)
            {
                lastIndex = i;
            }
            else
            {
                break;
            }
        }

        int index = Random.Range(0, lastIndex + 1);

        return ActiveFogs[index];
    }

    /// <summary>
    /// If there are more than one fog with level x, random one will be selected 
    /// </summary>
    public string GetUidOfFirstNotClearedFog()
    {
        return GetRandomActiveFogWithLeastLevel()?.Uid;
    }

    public List<GridMeshArea> GetFoggedAreas()
    {
        List<GridMeshArea> ret = new List<GridMeshArea>();

        // INCLUDE
        foreach (var pair in VisibleFogPositions)
        {
            var fogDef = pair.Value;

            FogObserver observer;
            if (FogObservers.TryGetValue(pair.Key, out observer))
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
}