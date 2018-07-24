using System.Collections.Generic;
using UnityEngine;

public class ChestDef
{
    private int pieceType = -1;
    
    public string Uid { get; set; }
    public int Time { get; set; }
    public CurrencyPair Price { get; set; }
    public int PieceAmount { get; set; }
    public List<ItemWeight> PieceWeights;
    public List<string> RandomPieces;
    
    public int Piece
    {
        get
        {
            if (pieceType == -1)
            {
                pieceType = PieceType.Parse(Uid);
            }
            
            return pieceType;
        }
    }

    public int GetRandomPiece()
    {
        if (RandomPieces == null || RandomPieces.Count == 0) return PieceType.None.Id;

        var id = RandomPieces[Random.Range(0, RandomPieces.Count)];

        return PieceType.Parse(id);
    }
}