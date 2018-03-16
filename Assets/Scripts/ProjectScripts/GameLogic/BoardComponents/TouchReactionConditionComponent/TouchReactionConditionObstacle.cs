public class TouchReactionConditionObstacle : TouchReactionConditionComponent
{
    public override bool Check(BoardPosition position, Piece piece)
    {
        return piece.Context.ObstaclesLogic.Check(position);
    }
}