public class HighlightTaskPointToPiece : TaskHighlightUsingArrow
{   
    protected override bool ShowArrow(TaskEntity task, float delay)
    {
        IHavePieceId taskPiece = task as IHavePieceId;
        if (taskPiece == null)
        {
            return false;
        }
        
        int pieceId = taskPiece.PieceId;
        return HighlightTaskPointToPieceHelper.FindAndPointToRandomPiece(pieceId);
    }
}