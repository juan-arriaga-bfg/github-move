public class AreaRecalculateObserver: IPieceBoardObserver, IECSComponent
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;
    
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

    public void OnRegisterEntity(ECSEntity entity)
    {
        
    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {
        
    }
}