public class PieceDef
{
    public string Uid { get; set; }
    public int Delay { get; set; }
    
    public string SpawnPiece { get; set; }
    public int SpawnAmount { get; set; }
    
    public CurrencyPair SpawnResources { get; set; }
    public CurrencyPair CreateReward { get; set; }
    
    public int Piece
    {
        get { return PieceType.Parse(Uid); }
    }
    
    public int SpawnPieceType
    {
        get { return PieceType.Parse(SpawnPiece); }
    }

    public override string ToString()
    {
        return string.Format("Uid: {0}, SpawnPiece {1}, SpawnAmount {2}, Delay {3}, SpawnResources: {4} - {5}, CreateReward: {6} - {7}", Uid, SpawnPiece, SpawnAmount, Delay,
            SpawnResources.Currency, SpawnResources.Amount, CreateReward.Currency, CreateReward.Amount);
    }

    public static PieceDef Default()
    {
        return new PieceDef
        {
            Uid = PieceType.None.Abbreviations[0],
            Delay = 1,
            SpawnResources = new CurrencyPair{Currency = PieceType.C1.Abbreviations[0], Amount = 1}
        };
    }
}