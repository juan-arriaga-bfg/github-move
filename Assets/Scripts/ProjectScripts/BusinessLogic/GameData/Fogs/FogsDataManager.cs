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
    
    public Dictionary<BoardPosition, FogDef> FogPositions;
    public List<BoardPosition> Completed = new List<BoardPosition>();
    
    public void Reload()
    {
        DefaultPieceWeights = null;
        Fogs = null;
        FogPositions = null;
        Completed = new List<BoardPosition>();
        LoadData(new ResourceConfigDataMapper<FogsDataManager>("configs/fogs.data", NSConfigsSettings.Instance.IsUseEncryption));
    }
    
    public void LoadData(IDataMapper<FogsDataManager> dataMapper)
    {
        dataMapper.LoadData((data, error)=> 
        {
            FogPositions = new Dictionary<BoardPosition, FogDef>();
            
            if (string.IsNullOrEmpty(error))
            {
                DefaultPieceWeights = data.DefaultPieceWeights;
                Fogs = data.Fogs;
                
                var save = ProfileService.Current.GetComponent<FieldDefComponent>(FieldDefComponent.ComponentGuid);

                if (save?.CompleteFogPositions != null)
                {
                    Completed = save.CompleteFogPositions;
                }
                
                foreach (var def in data.Fogs)
                {
                    if(Completed.FindIndex(position => position.Equals(def.GetCenter())) != -1) continue;
                    
                    FogPositions.Add(def.GetCenter(), def);
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
        return FogPositions.TryGetValue(key, out def) == false ? null : def;
    }

    public void RemoveFog(BoardPosition key)
    {
        if(FogPositions.ContainsKey(key) == false) return;
        
        Completed.Add(key);
        FogPositions.Remove(key);
    }

    public BoardPosition? GetFogPositionByUid(string uid)
    {
        foreach (var pair in FogPositions)
        {
            if (pair.Value.Uid == uid)
            {              
                return pair.Value.GetCenter();
            }
        }

        return null;
    }

    /// <summary>
    /// If there are more than one fog with level x, random one will be selected 
    /// </summary>
    public string GetUidOfFirstNotClearedFog()
    {
        List<FogDef> defs = new List<FogDef>();
        foreach (var pair in FogPositions)
        {
            if (Completed.Contains(pair.Key))
            {
                continue;
            }
            
            defs.Add(pair.Value);
        }

        if (defs.Count == 0)
        {
            Debug.LogError("[FogsDataManager] => GetUidOfFirstNotClearedFog: No defs found!");
            return null;
        }
        
        defs.Sort((def1, def2) => def1.Level - def2.Level);
        
        int firstLevel = defs[0].Level;
        int lastIndex = -1;
        
        for (int i = 1; i < defs.Count; i++)
        {
            var def = defs[i];
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

        string ret = defs[index].Uid;

        return ret;
    }

    public List<GridMeshArea> GetFoggedAreas()
    {
        List<GridMeshArea> ret = new List<GridMeshArea>();

        foreach (var fogDef in FogPositions.Values)
        {
            var positions = fogDef.Positions;

            var area = GetFogAreaForPositions(positions, false);

            ret.Add(area);
        }
        
        // Starting field
        List<BoardPosition> startPositions = new List<BoardPosition>();
        for (int x = 20; x <= 25; x++)
        {
            for (int y = 8; y <= 12; y++)
            {
                startPositions.Add(new BoardPosition(x, y));
            }
        }

        ret.Add(GetFogAreaForPositions(startPositions, true));
        
        return ret;
    }

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
}