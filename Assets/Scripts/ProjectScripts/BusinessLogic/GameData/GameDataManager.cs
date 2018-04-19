public class PieceWeight
{
    private int pieceType = -1;
    
    public string Uid { get; set; }
    public int Weight { get; set; }
    public bool Override { get; set; }
    
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

    public PieceWeight Copy()
    {
        return new PieceWeight{Uid = this.Uid, Weight = this.Weight};
    }

    public override string ToString()
    {
        return string.Format("Uid: {0} - Weight: {1} - Override: {2}", Uid, Weight, Override);
    }
}


public class GameDataManager
{
    public readonly ChestsDataManager ChestsManager = new ChestsDataManager();
    public readonly EnemiesDataManager EnemiesManager = new EnemiesDataManager();
    public readonly HeroesDataManager HeroesManager = new HeroesDataManager();
    public readonly PiecesDataManager PiecesManager = new PiecesDataManager();
    public readonly ObstaclesDataManager ObstaclesManager = new ObstaclesDataManager();
    public readonly SimpleObstaclesDataManager SimpleObstaclesManager = new SimpleObstaclesDataManager();
    public readonly QuestsDataManager QuestsManager = new QuestsDataManager();
    public readonly FogsDataManager FogsManager = new FogsDataManager();
}