using System.Collections.Generic;
using UnityEngine;

public class FogsDataManager : IDataLoader<List<FogDef>>
{
    public Dictionary<BoardPosition, FogDef> Fogs;
    
    public void LoadData(IDataMapper<List<FogDef>> dataMapper)
    {
        dataMapper.LoadData((data, error)=> 
        {
            Fogs = new Dictionary<BoardPosition, FogDef>();
            
            if (string.IsNullOrEmpty(error))
            {
                foreach (var def in data)
                {
                    Fogs.Add(def.Position, def);
                }
            }
            else
            {
                Debug.LogWarning("[FogsDataManager]: fogs config not loaded");
            }
        });
    }
}