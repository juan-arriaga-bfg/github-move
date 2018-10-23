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

    public BoardPosition? GetFogPositionByUid(string uid, bool includeCleared)
    {
        foreach (var pair in FogPositions)
        {
            if (pair.Value.Uid == uid)
            {
                var pos = pair.Key;
                if (!includeCleared && Completed.Contains(pos))
                {
                    break;
                }
                
                return pair.Value.GetCenter();
            }
        }

        return null;
    }
}