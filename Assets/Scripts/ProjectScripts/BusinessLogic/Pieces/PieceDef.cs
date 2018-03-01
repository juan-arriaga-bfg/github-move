public class PieceDef
{
    public string Uid { get; set; }
    public int Delay { get; set; }
    
    public string SpawnPiece { get; set; }
    public int SpawnAmount { get; set; }
    
    public CurrencyPair Resources { get; set; }
    public CurrencyPair CreateReward { get; set; }
    
    public int Piece
    {
        get { return PieceType.Parse(Uid); }
    }

    public override string ToString()
    {
        return string.Format("Uid: {0}, SpawnPiece {1}, SpawnAmount {2}, Delay {3}, Resources: {4} - {5}, CreateReward: {6} - {7}", Uid, SpawnPiece, SpawnAmount, Delay,
            Resources.Currency, Resources.Amount, CreateReward.Currency, CreateReward.Amount);
    }
}