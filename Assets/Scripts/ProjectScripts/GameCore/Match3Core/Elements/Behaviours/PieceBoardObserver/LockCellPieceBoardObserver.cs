public class LockCellPieceBoardObserver : IPieceBoardObserver
{
    public void OnAddToBoard(BoardPosition position, Piece context = null)
    {
        context.Context.BoardLogic.LockCell(position, this);
    }

    public void OnRemoveFromBoard(BoardPosition position, Piece context = null)
    {
        context.Context.BoardLogic.UnlockCell(position, this);
    }
}