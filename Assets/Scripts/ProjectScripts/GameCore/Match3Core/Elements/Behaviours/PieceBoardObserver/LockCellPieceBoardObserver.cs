public class LockCellPieceBoardObserver : IPieceBoardObserver
{
    public void OnAddToBoard(BoardPosition position, Piece context = null)
    {
        context.Context.BoardLogic.LockCell(position, this);
    }

    public void OnMovedFromTo(BoardPosition @from, BoardPosition to, Piece context = null)
    {
        
    }

    public void OnRemoveFromBoard(BoardPosition position, Piece context = null)
    {
        context.Context.BoardLogic.UnlockCell(position, this);
    }
}