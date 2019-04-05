public class CachedPiecePositionComponent : IECSComponent, IPieceBoardObserver
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	public int Guid => ComponentGuid;

	private Piece contextPiece;

	public void OnRegisterEntity(ECSEntity entity)
	{
		contextPiece = entity as Piece;

		var observer = contextPiece?.GetComponent<PieceBoardObserversComponent>(PieceBoardObserversComponent.ComponentGuid);
		observer?.RegisterObserver(this);
	}
	
	public void OnUnRegisterEntity(ECSEntity entity)
	{
		var observer = contextPiece?.GetComponent<PieceBoardObserversComponent>(PieceBoardObserversComponent.ComponentGuid);
		observer?.UnRegisterObserver(this);
	}

	public void OnAddToBoard(BoardPosition position, Piece context = null)
	{
		if (contextPiece == null) return;
		
		contextPiece.CachedPosition = position;
		AddPositionToDefaultCache(position, context);
	}

	public void OnMovedFromToStart(BoardPosition @from, BoardPosition to, Piece context = null)
	{
	}

	public void OnMovedFromToFinish(BoardPosition @from, BoardPosition to, Piece context = null)
	{
		if (contextPiece != null) contextPiece.CachedPosition = to;
		
		if(context == null) return; 
		
		RemovePositionFromDefaultCache(from, context);
		AddPositionToDefaultCache(to, context);
	}

	public void OnRemoveFromBoard(BoardPosition position, Piece context = null)
	{
		RemovePositionFromDefaultCache(position, context);
		RemovePositionFromPathfindCache();
	}

	private void RemovePositionFromDefaultCache(BoardPosition position, Piece context)
	{
		context?.Context.BoardLogic.PositionsCache.RemovePosition(context.PieceType, position);
	}

	private void AddPositionToDefaultCache(BoardPosition position, Piece context)
	{
		context.Context.BoardLogic.PositionsCache.AddPosition(context.PieceType, position);
	}

	private void RemovePositionFromPathfindCache()
	{
		var target = contextPiece?.Context.AreaAccessController?.AvailiablePositions;
		contextPiece?.Context.PathfindLocker?.RemoveFromCache(contextPiece);
	}
}
