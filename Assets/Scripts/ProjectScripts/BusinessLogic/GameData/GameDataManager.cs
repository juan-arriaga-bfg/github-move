using System.Collections.Generic;
using UnityEngine;

public class GameDataManager : IDataLoader<List<ChestDef>>
{
    public List<ChestDef> Chests;
    
    public void LoadData(IDataMapper<List<ChestDef>> dataMapper)
    {
        dataMapper.LoadData((data, error) =>
        {
            if (string.IsNullOrEmpty(error))
            {
                Chests = data;
            }
            else
            {
                Debug.LogWarning("[GameDataManager]: data config not loaded");
            }
        });
    }
}