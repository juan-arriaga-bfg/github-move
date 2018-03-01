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
		if (contextPiece != null) contextPiece.CachedPosition = position;
	}

	public void OnMovedFromTo(BoardPosition @from, BoardPosition to, Piece context = null)
	{
		if (contextPiece != null) contextPiece.CachedPosition = to;
	}

	public void OnRemoveFromBoard(BoardPosition position, Piece context = null)
	{
		
	}
}
