using System.Collections.Generic;
using System.Linq;
using System.Text;
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

    public FogObserver GetFogObserver(BoardPosition pos)
    {
        FogObserver ret;
        if (FogObservers.TryGetValue(pos, out ret))
        {
            return ret;
        }

        return null;
    }
    
    public bool IsFogCleared(string uid)
    {
        var pos = GetFogPositionByUid(uid);
        return !pos.HasValue;
    }

    public FogDef GetNextRandomFog()
    {
        List<FogObserver> fogsToSearch = VisibleFogPositions.Values.Select(e => GetFogObserver(e.GetCenter())).ToList();
        if (fogsToSearch.Count == 0)
        {
            Debug.LogError("[FogsDataManager] => GetNextRandomFog: No visible fog found!");
            return null;
        }

        // Sort fogs;
        List<FogObserver> sortedFogs = fogsToSearch.OrderByDescending(e => e.CanBeReached())
                                                   .ThenByDescending(e => e.RequiredLevelReached())
                                                   .ThenBy(e => e.Def.Condition.Amount)
                                                   .ThenBy(e => e.Def.Level)
                                                   .ToList();

        var firstFog = sortedFogs[0];

        List<FogObserver> selectedFogs = new List<FogObserver>
        {
            firstFog
        };

        for (var i = 1; i < sortedFogs.Count; i++)
        {
            var fog = sortedFogs[i];
            if (fog.CanBeReached() == firstFog.CanBeReached()
             && fog.RequiredLevelReached() == firstFog.RequiredLevelReached()
             && fog.Def.Condition.Amount == firstFog.Def.Condition.Amount
             && fog.Def.Level == firstFog.Def.Level)
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
//
//     private int CalculateFogWeightForSorting(FogDef fog)
//     {
//         const int WEIGHT_PATH   = 100000;
//         const int WEIGHT_BUBBLE = 10000;
//         const int WEIGHT_LEVEL  = 1000;
//
//         var observer = FogObservers[fog.GetCenter()];
//
//         int w = 0;
//         if (observer.CanBeReached()) { w += WEIGHT_PATH;}
//         if (observer.RequiredLevelReached()) { w += WEIGHT_LEVEL;}
//
//     }
//
// /// <summary>
//     /// Find fog with minimal level. Active fogs have top priority even when their level are higher
//     /// </summary>
//     /// <returns></returns>
//     public FogDef GetRandomFogWithLeastLevel()
//     {
//         List<FogDef> fogsToSearch = ActiveFogs.Count > 0 ? ActiveFogs : VisibleFogPositions.Values.ToList();
//
//         if (fogsToSearch == null || fogsToSearch.Count == 0)
//         {
//             Debug.LogError("[FogsDataManager] => GetRandomFogWithLeastLevel: No visible fog found!");
//             return null;
//         }
//
//         List<FogDef> fogsWithMinLevel = new List<FogDef>();
//
//         foreach (var fog in ActiveFogs)
//         {
//             if (fogsWithMinLevel.Count == 0)
//             {
//                 fogsWithMinLevel.Add(fog);
//                 continue;
//             }
//
//             var firstSavedFogLevel = fogsWithMinLevel[0].Level;
//             var currentFogLevel = fog.Condition.Amount;
//             
//             if (firstSavedFogLevel > currentFogLevel)
//             {
//                 fogsWithMinLevel.Clear();
//                 fogsWithMinLevel.Add(fog);
//             }
//             else if (firstSavedFogLevel == currentFogLevel)
//             {
//                 fogsWithMinLevel.Add(fog);
//             }
//         }
//
//         if (fogsWithMinLevel.Count == 0)
//         {
//             Debug.LogError("[FogsDataManager] => GetRandomFogWithLeastLevel: No defs with min level found!");
//             return null;
//         }
//         
//         int index = Random.Range(0, fogsWithMinLevel.Count);
//
//         return fogsWithMinLevel[index];
//     }
//     
//     /// <summary>
//     /// Find fog with minimal price. Active fogs have top priority even when their prices are higher
//     /// </summary>
//     /// <returns></returns>
//     public FogDef GetRandomFogWithLeastPrice()
//     {
//         List<FogDef> fogsToSearch = ActiveFogs.Count > 0 ? ActiveFogs : VisibleFogPositions.Values.ToList();
//
//         if (fogsToSearch == null || fogsToSearch.Count == 0)
//         {
//             Debug.LogError("[FogsDataManager] => GetRandomActiveFogWithLeastPrice: No visible fog found!");
//             return null;
//         }
//
//         List<FogDef> fogsWithMinPrice = new List<FogDef>();
//
//         foreach (var fog in ActiveFogs)
//         {
//             if (fogsWithMinPrice.Count == 0)
//             {
//                 fogsWithMinPrice.Add(fog);
//                 continue;
//             }
//
//             var firstSavedFogPrice = fogsWithMinPrice[0].Condition.Amount;
//             var currentFogPrice = fog.Condition.Amount;
//             
//             if (firstSavedFogPrice > currentFogPrice)
//             {
//                 fogsWithMinPrice.Clear();
//                 fogsWithMinPrice.Add(fog);
//             }
//             else if (firstSavedFogPrice == currentFogPrice)
//             {
//                 fogsWithMinPrice.Add(fog);
//             }
//         }
//
//         if (fogsWithMinPrice.Count == 0)
//         {
//             Debug.LogError("[FogsDataManager] => GetRandomActiveFogWithLeastPrice: No defs with least price found!");
//             return null;
//         }
//         
//         int index = Random.Range(0, fogsWithMinPrice.Count);
//
//         return fogsWithMinPrice[index];
//     }

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