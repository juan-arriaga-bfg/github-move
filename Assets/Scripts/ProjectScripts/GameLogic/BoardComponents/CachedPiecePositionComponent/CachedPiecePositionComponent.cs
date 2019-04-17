public class CachedPiecePositionComponent : IECSComponent, IPieceBoardObserver
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	public int Guid => ComponentGuid;

	private Piece contextPiece;

	public void OnRegisterEntity(ECSEntity entity)
	{
		contextPiece = entity as Piece;
	}
	
	public void OnUnRegisterEntity(ECSEntity entity)
	{
	}

	public void OnAddToBoard(BoardPosition position, Piece context = null)
	{
		contextPiece?.Context.BoardLogic.PositionsCache.AddPosition(contextPiece.PieceType, position);
	}

	public void OnMovedFromToStart(BoardPosition @from, BoardPosition to, Piece context = null)
	{
		
	}

	public void OnMovedFromToFinish(BoardPosition @from, BoardPosition to, Piece context = null)
	{
		contextPiece?.Context.BoardLogic.PositionsCache.RemovePosition(contextPiece.PieceType, from);
		contextPiece?.Context.BoardLogic.PositionsCache.AddPosition(contextPiece.PieceType, to);
	}

	public void OnRemoveFromBoard(BoardPosition position, Piece context = null)
	{
		contextPiece?.Context.BoardLogic.PositionsCache.RemovePosition(contextPiece.PieceType, position);
	}
}
