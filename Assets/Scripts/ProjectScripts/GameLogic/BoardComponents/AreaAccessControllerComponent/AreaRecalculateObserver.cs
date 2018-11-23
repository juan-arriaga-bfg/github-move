public class AreaRecalculateObserver: IPieceBoardObserver
{
    public void OnAddToBoard(BoardPosition position, Piece context = null)
    {
    }

    public void OnMovedFromToStart(BoardPosition @from, BoardPosition to, Piece context = null)
    {
    }

    public void OnMovedFromToFinish(BoardPosition @from, BoardPosition to, Piece context = null)
    {
    }

    public void OnRemoveFromBoard(BoardPosition position, Piece context = null)
    {
        var board = BoardService.Current.GetBoardById(0);
        var piece = board.BoardLogic.GetPieceAt(position);
        board.AreaAccessController?.LocalRecalculate(context.CachedPosition);
        
    }
}