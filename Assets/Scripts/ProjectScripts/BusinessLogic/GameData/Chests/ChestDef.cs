using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChestDef
{
    public string Uid;
    
    public int PieceAmount;
    
    public List<ItemWeight> PieceWeights;
    
    public AmountRange ProductionAmount;
    public AmountRange ResourcesAmount;
    public AmountRange CharactersAmount;
    
    private int pieceType = -1;
    public int Piece => pieceType == -1 ? (pieceType = PieceType.Parse(Uid)) : pieceType;
}