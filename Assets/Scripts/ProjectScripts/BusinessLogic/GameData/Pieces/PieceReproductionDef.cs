public class PieceReproductionDef : SimplePieceDef
{
    public int Limit;
    public string Obstacle;
    public CurrencyPair StepReward;
    public CurrencyPair Reproduction;

    private int obstacleType = -1;
    public int ObstacleType => obstacleType != -1 ? obstacleType : (obstacleType = PieceType.Parse(Obstacle));
}