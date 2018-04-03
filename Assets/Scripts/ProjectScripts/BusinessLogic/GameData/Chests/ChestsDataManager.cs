using System;
using System.Collections.Generic;
using UnityEngine;

public class ChestsDataManager : IDataLoader<List<ChestDef>>
{
    public const int Max = 4;
    
    public List<ChestDef> Chests;
    public List<Chest> ActiveChests = new List<Chest>();
    
    public void LoadData(IDataMapper<List<ChestDef>> dataMapper)
    {
        dataMapper.LoadData((data, error)=> 
        {
            Chests = new List<ChestDef>();
            
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

    public Chest GetChest(ChestType type)
    {
        var chest = Chests.Find(def => def.Uid == type.ToString());
        
        return new Chest(chest);
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

    public ChestType PieceToChest(int pieceType)
    {
        foreach (ChestType chest in Enum.GetValues(typeof(ChestType)))
        {
            if(PieceType.Parse(chest.ToString()) != pieceType) continue;
            
            return chest;
        }

        return ChestType.None;
    }

    public bool AddActiveChest(ChestType type)
    {
        if (ActiveChests.Count == Max) return false;
        
        ActiveChests.Add(GetChest(type));
        
        return true;
    }

    public bool RemoveActiveChest(Chest chest)
    {
        for (var i = ActiveChests.Count - 1; i >= 0; i--)
        {
            if(ActiveChests[i] != chest) continue;
            
            ActiveChests.RemoveAt(i);
            return true;
        }
        
        return false;
    }
}