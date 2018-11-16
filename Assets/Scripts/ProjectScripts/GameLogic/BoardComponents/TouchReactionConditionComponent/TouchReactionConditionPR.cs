public class TouchReactionConditionPR : TouchReactionConditionStorage
{
    public override bool Check(BoardPosition position, Piece piece)
    {
        var save = BoardService.Current.FirstBoard.TutorialLogic.Save;

        return save.Contains(TutorialBuilder.LockPRStepIndex) && base.Check(position, piece);
    }
}