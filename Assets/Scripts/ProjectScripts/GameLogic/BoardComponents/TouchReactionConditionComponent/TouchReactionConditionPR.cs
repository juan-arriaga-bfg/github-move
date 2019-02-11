public class TouchReactionConditionPR : TouchReactionConditionWorkplace
{
    public override bool Check(BoardPosition position, Piece piece)
    {
        return piece.Context.TutorialLogic.CheckLockPR() && base.Check(position, piece);
    }
}