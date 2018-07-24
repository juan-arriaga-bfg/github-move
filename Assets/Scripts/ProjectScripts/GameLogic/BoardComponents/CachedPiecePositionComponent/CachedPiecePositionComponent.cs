public class CachedPiecePositionComponent : IECSComponent, IPieceBoardObserver
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	
	public int Guid { get { return ComponentGuid; } }

	private Piece contextPiece;

	public void OnRegisterEntity(ECSEntity entity)
	{
		contextPiece = entity as Piece;

		if (contextPiece != null)
		{
			var observer = contextPiece.GetComponent<PieceBoardObserversComponent>(PieceBoardObserversComponent.ComponentGuid);
			if (observer != null)
			{
				observer.RegisterObserver(this);
			}
		}
	}
	
	public void OnUnRegisterEntity(ECSEntity entity)
	{
		if (contextPiece != null)
		{
			var observer = contextPiece.GetComponent<PieceBoardObserversComponent>(PieceBoardObserversComponent.ComponentGuid);
			if (observer != null)
			{
				observer.UnRegisterObserver(this);
			}
		}
	}

	public void OnAddToBoard(BoardPosition position, Piece context = null)
	{
		if (contextPiece == null) return;
		
		contextPiece.CachedPosition = position;
		GameDataService.Current.PiecesManager.CachedPosition(context, position);
		
		if(context == null) return; 
		
		context.Context.BoardLogic.PositionsCache.AddPosition(context.PieceType, position);
	}

	public void OnMovedFromTo(BoardPosition @from, BoardPosition to, Piece context = null)
	{
		if (contextPiece != null) contextPiece.CachedPosition = to;
		
		if(context == null) return; 
		
		context.Context.BoardLogic.PositionsCache.RemovePosition(context.PieceType, from);
		context.Context.BoardLogic.PositionsCache.AddPosition(context.PieceType, to);
	}

	public void OnRemoveFromBoard(BoardPosition position, Piece context = null)
	{
		if(context == null) return; 
		
		context.Context.BoardLogic.PositionsCache.RemovePosition(context.PieceType, position);
	}
}
