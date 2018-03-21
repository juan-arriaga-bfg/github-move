using System;
using System.Collections.Generic;
using UnityEngine;

public class ChestsDataManager : IDataLoader<List<ChestDef>>
{
    public List<Chest> Chests;
    
    public void LoadData(IDataMapper<List<ChestDef>> dataMapper)
    {
        dataMapper.LoadData((data, error)=> 
        {
            Chests = new List<Chest>();
            
            if (string.IsNullOrEmpty(error))
            {
                foreach (var def in data)
                {
                    Chests.Add(new Chest(def));
                }
            }
            else
            {
                Debug.LogWarning("[ChestsDataManager]: chests config not loaded");
            }
        });
    }

    public Chest GetChest(ChestType type)
    {
        return Chests.Find(chest => chest.ChestType == type);
    }

    public Chest GetChest(int pieceType)
    {
        foreach (ChestType chest in Enum.GetValues(typeof(ChestType)))
        {
            if(PieceType.Parse(chest.ToString()) != pieceType) continue;
            
            return GetChest(chest);
        }
        
        return null;
    }
}