public class HighlightTaskPointToPredecessor : TaskHighlightUsingArrow
{   
    protected override bool ShowArrow(TaskEntity task, float delay)
    {
        IHavePieceId taskPiece = task as IHavePieceId;
        if (taskPiece == null)
        {
            return false;
        }

        if (taskPiece.PieceId == PieceType.Empty.Id || taskPiece.PieceId == PieceType.None.Id)
        {
            return false;
        }
        
        int pieceId = taskPiece.PieceId;
        return HighlightTaskPointToPieceHelper.FindAndPointToRandomPredecessorPiece(pieceId);
    }
}