public class TouchReactionConditionPR : TouchReactionConditionStorage
{
    public override bool Check(BoardPosition position, Piece piece)
    {
        var save = BoardService.Current.FirstBoard.TutorialLogic.Save;

        return save.Contains(8) && base.Check(position, piece);
    }
}