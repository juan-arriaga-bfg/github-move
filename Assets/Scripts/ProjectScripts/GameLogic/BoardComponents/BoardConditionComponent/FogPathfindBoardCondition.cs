public class FogPathfindBoardCondition : PiecePathfindBoardCondition
{
    public FogPathfindBoardCondition(BoardController context, Piece piece) : base(context, piece)
    {
    }
    
    public override bool Check(BoardPosition position)
    {
        if (!CheckMapLimits(position)) return false;

        var boardLogic = Context.BoardLogic;
        var pieceInCurrentPos = boardLogic.GetPieceAt(position);
        
        return pieceInCurrentPos == Piece || !boardLogic.IsLockedCell(position) && PathfindIgnore.CanIgnore(pieceInCurrentPos);
    }
}