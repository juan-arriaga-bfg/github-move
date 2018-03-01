public class CachedPiecePositionComponent : IECSComponent, IPieceBoardObserver
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	
	public int Guid { get { return ComponentGuid; } }

	private Piece context;

	public void OnRegisterEntity(ECSEntity entity)
	{
		context = entity as Piece;

		if (context != null)
		{
			var observer = context.GetComponent<PieceBoardObserversComponent>(PieceBoardObserversComponent.ComponentGuid);
			if (observer != null)
			{
				observer.RegisterObserver(this);
			}
		}
	}
	
	public void OnUnRegisterEntity(ECSEntity entity)
	{
		if (context != null)
		{
			var observer = context.GetComponent<PieceBoardObserversComponent>(PieceBoardObserversComponent.ComponentGuid);
			if (observer != null)
			{
				observer.UnRegisterObserver(this);
			}
		}
	}

	public void OnAddToBoard(BoardPosition position, Piece context = null)
	{
		if (context != null) context.CachedPosition = position;
	}

	public void OnMovedFromTo(BoardPosition @from, BoardPosition to, Piece context = null)
	{
		if (context != null) context.CachedPosition = to;
	}

	public void OnRemoveFromBoard(BoardPosition position, Piece context = null)
	{
		
	}
}
