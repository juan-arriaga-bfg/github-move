using System.Collections.Generic;

public class MulticellularPieceBoardObserver : IECSComponent, IPieceBoardObserver
{
	public static int ComponentGuid = ECSManager.GetNextGuid();
    
	public int Guid
	{
		get { return ComponentGuid; }
	}
	
	public List<BoardPosition> Mask;
	protected BoardPosition realPosition;
	
	public void OnRegisterEntity(ECSEntity entity)
	{
	}

	public void OnUnRegisterEntity(ECSEntity entity)
	{
	}
	
	public virtual void OnAddToBoard(BoardPosition position, Piece context = null)
	{
		if(context == null) return;
		
		realPosition = position;
		
		for (int i = 0; i < Mask.Count; i++)
		{
			var point = GetPointInMask(position, Mask[i]);
			context.Context.BoardLogic.AddPieceToBoardSilent(point.X, point.Y, context);
		}
	}

	public void OnMovedFromTo(BoardPosition @from, BoardPosition to, Piece context = null)
	{
		
	}

	public virtual void OnRemoveFromBoard(BoardPosition position, Piece context = null)
	{
		if(context == null || realPosition.Equals(position) == false) return;
		
		for (int i = 0; i < Mask.Count; i++)
		{
			var point = GetPointInMask(position, Mask[i]);
			context.Context.BoardLogic.RemovePieceFromBoardSilent(point);
		}
	}

	protected BoardPosition GetPointInMask(BoardPosition position, BoardPosition mask)
	{
		return new BoardPosition(position.X + mask.X, position.Y + mask.Y, position.Z);
	}
}