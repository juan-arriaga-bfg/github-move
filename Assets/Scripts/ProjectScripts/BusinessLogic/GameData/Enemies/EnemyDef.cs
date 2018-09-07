public class EnemyDef
{ 
    private int pieceType = -1;

    public string Uid;

    public CurrencyPair Price;
    
    public CurrencyPair Reward;

    public string Name;
    
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