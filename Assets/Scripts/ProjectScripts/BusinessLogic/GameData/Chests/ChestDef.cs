using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChestDef
{
    private int pieceType = -1;
    
    private Dictionary<int, int> hardPieces;
    
    public string Uid { get; set; }
    public int Time { get; set; }
    public CurrencyPair Price { get; set; }
    public int PieceAmount { get; set; }
    
    public List<ItemWeight> PieceWeights;

    public int Piece => pieceType == -1 ? (pieceType = PieceType.Parse(Uid)) : pieceType;
}