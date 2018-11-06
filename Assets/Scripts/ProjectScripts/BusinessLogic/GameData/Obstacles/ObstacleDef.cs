using System.Collections.Generic;

public class ObstacleDef
{ 
    private int pieceType = -1;
    private int chestType = -1;

    public string Uid = "";
    public string ChestId = "";
    
    public int Delay { get; set; }
    
    public CurrencyPair Price { get; set; }
    public List<CurrencyPair> StepRewards { get; set; }
    
    public int PieceAmount { get; set; }
    public List<ItemWeight> PieceWeights { get; set; }
    
    public AmountRange ProductionAmount;
    
    public int Piece => pieceType == -1 ? (pieceType = PieceType.Parse(Uid)) : pieceType;
    public int Chest => chestType == -1 ? (chestType = PieceType.Parse(ChestId)) : chestType;
}