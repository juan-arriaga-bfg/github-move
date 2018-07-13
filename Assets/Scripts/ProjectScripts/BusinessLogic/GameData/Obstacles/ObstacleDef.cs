using System.Collections.Generic;

public class ObstacleDef
{ 
    private int pieceType = -1;
    
    public string Uid { get; set; }
    
    public int Delay { get; set; }
    
    public CurrencyPair Price { get; set; }
    public CurrencyPair FastPrice { get; set; }
    
    public int PieceAmount { get; set; }
    public List<ItemWeight> PieceWeights { get; set; }
    public List<ItemWeight> ChestWeights { get; set; }
    
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