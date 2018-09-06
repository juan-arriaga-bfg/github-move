public class PathfindLockObserver: IECSComponent, IPieceBoardObserver
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;

    protected Piece piece;
    protected BoardController board;
    
    public virtual void OnRegisterEntity(ECSEntity entity)
    {
        piece = entity as Piece;
        board = piece.Context;
    }

    public virtual void OnUnRegisterEntity(ECSEntity entity)
    {
    }

    public void OnAddToBoard(BoardPosition position, Piece context = null)
    {
        var target = GetTargetPosition();
       board.PathfindLocker?.RecalcCacheOnPieceAdded(target, position);
    }

    public void OnMovedFromToStart(BoardPosition @from, BoardPosition to, Piece context = null)
    {
    }

    public void OnMovedFromToFinish(BoardPosition @from, BoardPosition to, Piece context = null)
    {
        var target = GetTargetPosition();
        board.PathfindLocker?.RecalcCacheOnPieceMoved(target, from, to);
    }

    public void OnRemoveFromBoard(BoardPosition position, Piece context = null)
    {
        var target = GetTargetPosition();
        board.PathfindLocker?.RecalcCacheOnPieceRemoved(target, position, piece);
    }

    private BoardPosition GetTargetPosition()
    {
        var target = board.BoardLogic.PositionsCache.GetRandomPositions(PieceType.Char1.Id, 1)[0];
        return target;
    }
}