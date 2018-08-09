public class CachedPiecePositionComponent : IECSComponent, IPieceBoardObserver
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	
	public int Guid { get { return ComponentGuid; } }

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
		GameDataService.Current.PiecesManager.CachedPosition(context, position);

		context?.Context.BoardLogic.PositionsCache.AddPosition(context.PieceType, position);
	}

	public void OnMovedFromToStart(BoardPosition @from, BoardPosition to, Piece context = null)
	{
	}

	public void OnMovedFromToFinish(BoardPosition @from, BoardPosition to, Piece context = null)
	{
		if (contextPiece != null) contextPiece.CachedPosition = to;
		
		if(context == null) return; 
		
		context.Context.BoardLogic.PositionsCache.RemovePosition(context.PieceType, from);
		context.Context.BoardLogic.PositionsCache.AddPosition(context.PieceType, to);
	}

	public void OnRemoveFromBoard(BoardPosition position, Piece context = null)
	{
		context?.Context.BoardLogic.PositionsCache.RemovePosition(context.PieceType, position);
	}
}
