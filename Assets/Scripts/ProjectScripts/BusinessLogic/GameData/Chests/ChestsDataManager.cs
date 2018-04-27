using System.Collections.Generic;
using UnityEngine;

public class ChestsDataManager : IDataLoader<List<ChestDef>>
{
    public const int Max = 4;

    private int index = -1;

    public Chest ActiveChest;
    public List<ChestDef> Chests;
    
    public List<Chest> ActiveChests = new List<Chest>();
    
    public readonly Dictionary<BoardPosition, Chest> ChestsOnBoard = new Dictionary<BoardPosition, Chest>();
    
    public void LoadData(IDataMapper<List<ChestDef>> dataMapper)
    {
        dataMapper.LoadData((data, error)=> 
        {
            if (string.IsNullOrEmpty(error))
            {
                Chests = new List<ChestDef> {data[0]};
                
                for (var i = 1; i < data.Count; i++)
                {
                    var def = data[i - 1];
                    var defNext = data[i];
                    
                    defNext.PieceWeights = ItemWeight.ReplaseWeights(def.PieceWeights, defNext.PieceWeights);
                    defNext.ChargerWeights = ItemWeight.ReplaseWeights(def.ChargerWeights, defNext.ChargerWeights);
                    
                    Chests.Add(defNext);
                }
            }
            else
            {
                Debug.LogWarning("[ChestsDataManager]: chests config not loaded");
            }
        });
    }
    
    public Chest GetChest(int pieceType)
    {
        var chestDef = Chests.Find(def => def.Piece == pieceType);
        
        return chestDef == null ? null : new Chest(chestDef);
    }
    
    public bool AddToBoard(BoardPosition position, int pieceType, bool isOpen = false)
    {
        if (ChestsOnBoard.ContainsKey(position)) return false;

        var chest = GetChest(pieceType);

        if (isOpen) chest.State = ChestState.Open;
        
        ChestsOnBoard.Add(position, chest);
        
        return true;
    }
    
    public Chest GetFromBoard(BoardPosition position, int pieceType)
    {
        Chest chest;

        if (ChestsOnBoard.TryGetValue(position, out chest))
        {
            ChestsOnBoard.Remove(position);
            return chest;
        }
        
        return GetChest(pieceType);
    }
    
    public bool AddActiveChest(int pieceType)
    {
        if (ActiveChests.Count == Max) return false;
        
        ActiveChests.Add(GetChest(pieceType));
        
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