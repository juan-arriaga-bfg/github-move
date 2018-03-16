using System.Collections.Generic;
using UnityEngine;

public class ChestsDataManager : IDataLoader<List<ChestDef>>
{
    public List<ChestDef> Chests;
    
    private readonly List<ChestDef> activeChests = new List<ChestDef>();
    
    public void LoadData(IDataMapper<List<ChestDef>> dataMapper)
    {
        dataMapper.LoadData((data, error)=> 
        {
            if (string.IsNullOrEmpty(error))
            {
                Chests = data;
            }
            else
            {
                Debug.LogWarning("[ChestsDataManager]: chests config not loaded");
            }
        });
    }
    
    public bool AddActiveChest(ChestDef chest)
    {
        if (activeChests.Count == 4) return false;

        chest.State = ChestState.Lock;
        activeChests.Add(chest);
        
        return true;
    }

    public void RemoveActiveChest(ChestDef chest)
    {
        var index = activeChests.IndexOf(chest);
        
        if(index == -1) return;
        
        activeChests.RemoveAt(index);
    }
    
    public List<ChestDef> GetActiveChests()
    {
        return activeChests;
    }

    public ChestDef GetChest(ChestType type)
    {
        return Chests.Find(def => def.GetChestType() == type);
    }
}