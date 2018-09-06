public class PathfindLockObserver: IECSComponent, IPieceBoardObserver
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;

    protected Piece piece;
    protected BoardController board;

    public bool AutoLock = true;
    
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
       if(target != null)
           board.PathfindLocker?.RecalcCacheOnPieceAdded(target.Value, position, AutoLock);
    }

    public void OnMovedFromToStart(BoardPosition @from, BoardPosition to, Piece context = null)
    {
    }

    public void OnMovedFromToFinish(BoardPosition @from, BoardPosition to, Piece context = null)
    {
        var target = GetTargetPosition();
        if(target != null)
            board.PathfindLocker?.RecalcCacheOnPieceMoved(target.Value, from, to, AutoLock);
    }

    public void OnRemoveFromBoard(BoardPosition position, Piece context = null)
    {
        var target = GetTargetPosition();
        if(target != null)
            board.PathfindLocker?.RecalcCacheOnPieceRemoved(target.Value, position, piece);
    }

    private BoardPosition? GetTargetPosition()
    {
        var pieces = board.BoardLogic.PositionsCache.GetRandomPositions(PieceType.Char1.Id, 1);
        if (pieces.Count > 0)
        {
            var target = pieces[0];
            return target;    
        }

        return null;
    }
}