using System;
using System.Collections.Generic;
using UnityEngine;

public class ChestsDataManager : IDataLoader<List<ChestDef>>
{
    public const int Max = 4;

    private int index = -1;
    
    public List<ChestDef> Chests;
    public List<Chest> ActiveChests = new List<Chest>();
    
    public readonly Dictionary<BoardPosition, Chest> ChestsOnBoard = new Dictionary<BoardPosition, Chest>();
    
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

    public bool AddToBoard(BoardPosition position, ChestType type, bool isOpen = false)
    {
        if (ChestsOnBoard.ContainsKey(position)) return false;

        var chest = GetChest(type);

        if (isOpen) chest.State = ChestState.Open;
        
        ChestsOnBoard.Add(position, chest);
        
        return true;
    }

    public bool MovedFromToBoard(BoardPosition from, BoardPosition to)
    {
        Chest chest;

        if (ChestsOnBoard.TryGetValue(from, out chest) == false || ChestsOnBoard.ContainsKey(to)) return false;
        
        ChestsOnBoard.Remove(from);
        ChestsOnBoard.Add(to, chest);
        
        return true;
    }
    
    public Chest GetFromBoard(BoardPosition position)
    {
        Chest chest;
        
        return ChestsOnBoard.TryGetValue(position, out chest) == false ? null : chest;
    }

    public bool RemoveFromBoard(BoardPosition position)
    {
        if (ChestsOnBoard.ContainsKey(position) == false) return false;

        ChestsOnBoard.Remove(position);
        
        return true;
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

    public void NextFreeChest()
    {
        index++;

        if (index == Chests.Count)
        {
            index = 0;
        }
    }
    
    public Chest GetFreeChest()
    {
        if (index == -1) index = 0;
        
        var type = (ChestType) Enum.Parse(typeof(ChestType), Chests[index].Uid);
        
        return GetChest(type);
    }
}