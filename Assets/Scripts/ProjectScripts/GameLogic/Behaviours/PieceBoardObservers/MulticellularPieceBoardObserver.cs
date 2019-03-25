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

    public virtual Vector3 GetWorldCenterPosition()
    {
        var center = Vector3.zero;
        
        for (int j = 0; j < Mask.Count; j++)
        {
            var maskPosition = Mask[j];
            var realPosition = GetPointInMask(this.realPosition, maskPosition);
            var maskLocalPosition = Context.Context.BoardDef.GetWorldPosition(realPosition.X, realPosition.Y);

            center = center + maskLocalPosition;
        }

        center = center / Mask.Count;

        return center;
    }

    public int GetWidth()
    {
        int defaultDimension = Mask[0].X;
        int width = 0;
            
        for (int i = 0; i < Mask.Count; i++)
        {
            var maskPosition = Mask[i];
            if (defaultDimension == maskPosition.X)
            {
                width = width + 1;
            }
        }
        return width > 0 ? width : 1;
    }

    public int GetHeight()
    {
        int defaultDimension = Mask[0].Y;
        int height = 0;
            
        for (int i = 0; i < Mask.Count; i++)
        {
            var maskPosition = Mask[i];
            if (defaultDimension == maskPosition.Y)
            {
                height = height + 1;
            }
        }
        return height > 0 ? height : 1;
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
    
    public BoardPosition GetUpPosition
    {
        get
        {
            int minX = 10000;
            int maxY = 0;
            BoardPosition targetPosition = realPosition;
            foreach (var cell in Mask)
            {
                var point = GetPointInMask(realPosition, cell);
                if (point.X <= minX && point.Y >= maxY)
                {
                    targetPosition = point;
                    minX = point.X;
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