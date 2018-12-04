using System.Collections.Generic;
using UnityEngine;

public class MulticellularPieceBoardObserver : IECSComponent, IPieceBoardObserver
{
	public static int ComponentGuid = ECSManager.GetNextGuid();
	public int Guid => ComponentGuid;

	public List<BoardPosition> Mask;

	public BoardPosition GetTopPosition
	{
		get
		{
			var size = (int)Mathf.Sqrt(Mask.Count);
			return realPosition.UpAtDistance(size - 1);
		}
	}
    
    public BoardPosition GetRightPosition
    {
        get
        {
            int maxX = 0;
            int maxY = 0;
            BoardPosition targetPosition = realPosition;
            foreach (var cell in Mask)
            {
                var point = GetPointInMask(realPosition, cell);
                if (point.X >= maxX && point.Y >= maxY)
                {
                    targetPosition = point;
                    maxX = point.X;
                    maxY = point.Y;
                }
            }

            return targetPosition;
        }
    }
	
	protected BoardPosition realPosition;
	public Piece Context;
	
	public void OnRegisterEntity(ECSEntity entity)
	{
		Context = entity as Piece;
	}

	public void OnUnRegisterEntity(ECSEntity entity)
	{
	}
	
	public virtual void OnAddToBoard(BoardPosition position, Piece context = null)
	{
		if(context == null) return;
		
		realPosition = position;
		
		foreach (var cell in Mask)
		{
			var point = GetPointInMask(position, cell);
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

		foreach (var cell in Mask)
		{
			var point = GetPointInMask(position, cell);
			context.Context.BoardLogic.RemovePieceFromBoardSilent(point);
		}
	}

	public virtual BoardPosition GetPointInMask(BoardPosition position, BoardPosition mask)
	{
		return new BoardPosition(position.X + mask.X, position.Y + mask.Y, position.Z);
	}
}