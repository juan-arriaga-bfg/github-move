using System.Collections.Generic;
using UnityEngine;

public class FogsDataManager : IDataLoader<FogsDataManager>
{
    public List<ItemWeight> DefaultPieceWeights { get; set; }
    public List<FogDef> Fogs { get; set; }
    
    public Dictionary<BoardPosition, FogDef> fogs;
    
    public void LoadData(IDataMapper<FogsDataManager> dataMapper)
    {
        dataMapper.LoadData((data, error)=> 
        {
            fogs = new Dictionary<BoardPosition, FogDef>();
            
            if (string.IsNullOrEmpty(error))
            {
                DefaultPieceWeights = data.DefaultPieceWeights;
                Fogs = data.Fogs;
                
                foreach (var def in data.Fogs)
                {
                    fogs.Add(def.Position, def);
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
        return fogs.TryGetValue(key, out def) == false ? null : def;
    }
}