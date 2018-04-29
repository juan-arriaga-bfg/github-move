using System.Collections.Generic;
using UnityEngine;

public class FogsDataManager : IDataLoader<FogsDataManager>
{
    public List<ItemWeight> DefaultPieceWeights { get; set; }
    public List<FogDef> Fogs { get; set; }
    
    public Dictionary<BoardPosition, FogDef> FogPositions;
    
    public void LoadData(IDataMapper<FogsDataManager> dataMapper)
    {
        dataMapper.LoadData((data, error)=> 
        {
            FogPositions = new Dictionary<BoardPosition, FogDef>();
            
            if (string.IsNullOrEmpty(error))
            {
                DefaultPieceWeights = data.DefaultPieceWeights;
                Fogs = data.Fogs;
                
                foreach (var def in data.Fogs)
                {
                    FogPositions.Add(def.Position, def);
                }
            }
            else
            {
                Debug.LogWarning("[FogsDataManager]: fogs config not loaded");
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

        FogPositions.Remove(key);
    }
}