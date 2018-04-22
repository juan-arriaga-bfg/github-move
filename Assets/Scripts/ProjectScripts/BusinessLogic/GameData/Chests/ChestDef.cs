using System;
using System.Collections.Generic;

public class ChestDef
{
    private int pieceType = -1;
    
    public string Uid { get; set; }
    public int Time { get; set; }
    public int MergePoints { get; set; }
    public CurrencyPair Price { get; set; }
    public List<int> PieceAmounts { get; set; }
    public List<int> ChargerAmounts { get; set; }
    public List<ItemWeight> PieceWeights;
    public List<ItemWeight> ChargerWeights;
    
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
}