public class TouchReactionConditionPR : TouchReactionConditionStorage
{
    public override bool Check(BoardPosition position, Piece piece)
    {
        var save = BoardService.Current.FirstBoard.TutorialLogic.Save;

        return save.Contains(9) && base.Check(position, piece);
    }
}