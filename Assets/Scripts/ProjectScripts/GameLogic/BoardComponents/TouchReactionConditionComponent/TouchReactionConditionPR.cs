public class TouchReactionConditionPR : TouchReactionConditionWorkplace
{
    public override bool Check(BoardPosition position, Piece piece)
    {
        return BoardService.Current.FirstBoard.TutorialLogic.CheckLockPR() && base.Check(position, piece);
    }
}