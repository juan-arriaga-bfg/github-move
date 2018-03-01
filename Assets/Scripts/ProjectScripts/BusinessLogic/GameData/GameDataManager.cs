using System.Collections.Generic;
using UnityEngine;

public class GameDataManager : IDataLoader<List<ChestDef>>
{
    public List<ChestDef> Chests;
    
    private List<ChestDef> activeChests;
    
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

    public bool AddActiveChest(ChestDef chest)
    {
        if (activeChests.Count == 4) return false;
        
        activeChests.Add(chest);
        
        return true;
    }
    
    public List<ChestDef> GetActiveChests()
    {
        return activeChests;
    }
}