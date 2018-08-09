using System.Collections.Generic;
using UnityEngine;

public class MulticellularPieceBoardObserver : IECSComponent, IPieceBoardObserver
{
	public static int ComponentGuid = ECSManager.GetNextGuid();
    
	public int Guid
	{
		get { return ComponentGuid; }
	}
	
	public List<BoardPosition> Mask;

	public BoardPosition GetTopPosition
	{
		get
		{
			var size = (int)Mathf.Sqrt(Mask.Count);
			return realPosition.UpAtDistance(size - 1);
		}
	}
	
	protected BoardPosition realPosition;
	protected Piece thisContext;
	
	public void OnRegisterEntity(ECSEntity entity)
	{
		thisContext = entity as Piece;
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

	public void OnMovedFromToStart(BoardPosition @from, BoardPosition to, Piece context = null)
	{
	}

	public void OnMovedFromToFinish(BoardPosition @from, BoardPosition to, Piece context = null)
	{
		realPosition = to;
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

	public BoardPosition GetPointInMask(BoardPosition position, BoardPosition mask)
	{
		return new BoardPosition(position.X + mask.X, position.Y + mask.Y, position.Z);
	}
}