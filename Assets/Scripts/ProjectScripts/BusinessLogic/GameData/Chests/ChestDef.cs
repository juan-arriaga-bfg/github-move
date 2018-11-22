using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChestDef
{
    public string Uid;
    public int Time;
    public CurrencyPair Price;
    
    public int PieceAmount;
    public List<ItemWeight> PieceWeights;
    
    public AmountRange ProductionAmount;
    public AmountRange ResourcesAmount;
    
    private int pieceType = -1;
    public int Piece => pieceType == -1 ? (pieceType = PieceType.Parse(Uid)) : pieceType;
}