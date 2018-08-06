using System.Collections.Generic;

public class PieceBoardObserversComponent : IECSComponent, IPieceBoardObserver
{
	public static int ComponentGuid = ECSManager.GetNextGuid();

	public int Guid
	{
		get { return ComponentGuid; }
	}

	protected Piece context;

	public virtual void OnRegisterEntity(ECSEntity entity)
	{
		this.context = entity as Piece;
	}

	public virtual void OnUnRegisterEntity(ECSEntity entity)
	{
	}

	protected List<IPieceBoardObserver> observers = new List<IPieceBoardObserver>();

	public virtual PieceBoardObserversComponent RegisterObserver(IPieceBoardObserver observer)
	{
		observers.Add(observer);

		return this;
	}
	
	public virtual PieceBoardObserversComponent UnRegisterObserver(IPieceBoardObserver observer)
	{
		observers.Remove(observer);
		
		return this;
	}

	public virtual void OnAddToBoard(BoardPosition position, Piece targetContext = null)
	{
		for (int i = 0; i < observers.Count; i++)
		{
			var observer = observers[i];
			observer.OnAddToBoard(position, context);
		}
	}

	public void OnMovedFromToStart(BoardPosition @from, BoardPosition to, Piece targetContext = null)
	{
		for (int i = 0; i < observers.Count; i++)
		{
			var observer = observers[i];
			observer.OnMovedFromToStart(from, to, context);
		}
	}

	public void OnMovedFromToFinish(BoardPosition @from, BoardPosition to, Piece targetContext = null)
	{
		for (int i = 0; i < observers.Count; i++)
		{
			var observer = observers[i];
			observer.OnMovedFromToFinish(from, to, context);
		}
	}

	public virtual void OnRemoveFromBoard(BoardPosition position, Piece targetContext = null)
	{
		for (int i = 0; i < observers.Count; i++)
		{
			var observer = observers[i];
			observer.OnRemoveFromBoard(position, context);
		}
	}
}