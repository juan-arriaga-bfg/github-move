﻿public abstract class DestroyObserverPieceComponent : IECSComponent, IDestroyPieceObserverCopy
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

	public abstract void OnDestroyPieceStart(BoardPosition at, BoardPosition current);
	
	public abstract void OnDestroyPieceFinish(BoardPosition at, BoardPosition current);
}