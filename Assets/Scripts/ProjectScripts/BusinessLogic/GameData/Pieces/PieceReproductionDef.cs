public class PieceReproductionDef : SimplePieceDef
{
    public int Limit;
    public string Obstacle;
    public CurrencyPair StepReward;
    public string ReproductionId;
    public AmountRange ReproductionRange;

    private int obstacleType = -1;
    public int ObstacleType => obstacleType != -1 ? obstacleType : (obstacleType = PieceType.Parse(Obstacle));
}