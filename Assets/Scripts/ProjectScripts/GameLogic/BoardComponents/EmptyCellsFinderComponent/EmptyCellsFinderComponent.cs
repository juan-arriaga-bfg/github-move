using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EmptyCellsFinderComponent : IECSComponent
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	public int Guid => ComponentGuid;

	private BoardLogicComponent context;
	
	public void OnRegisterEntity(ECSEntity entity)
	{
		this.context = entity as BoardLogicComponent;
	}
	
	public void OnUnRegisterEntity(ECSEntity entity)
	{
	}

	public bool CheckInFrontOrFindRandomNear(BoardPosition point, List<BoardPosition> field, int amount)
	{
		if (amount == 1)
		{
			if (AddIsEmpty(point.Down, field, amount)) return true;
		}
		
		if (context.EmptyCellsFinder.FindRandomNearWithPointInCenter(point, field, amount * 4) == false) return false;

		var inFront = field.FindAll(position => position.Y < point.Y);

		if (inFront.Count >= amount)
		{
			inFront.RemoveRange(amount, inFront.Count - amount);
			field.Clear();
			field.AddRange(inFront);
		}
		else if (field.Count > amount)
		{
			field.RemoveRange(amount, field.Count - amount);
		}
		
		return field.Count > 0;
	}
	
	public bool FindRandomNearWithPointInCenter(BoardPosition point, List<BoardPosition> field, int amount, float extraSpacePercent = 0)
	{
		var index = 0;
		extraSpacePercent = Mathf.Clamp(extraSpacePercent, 0, float.MaxValue);
		var extra = 1 + extraSpacePercent;

		while (field.Count < amount * extra && index < 10)
		{
			index++;
			FindRingWithPointInCenter(point, field, (index * 2) * 4, index);
		}

		if (field.Count == 0)
		{
			return false;
		}

		field.Shuffle();
		
		if (field.Count > amount)
		{
			field.RemoveRange(amount, field.Count - amount);
		}

		return field.Count != 0;
	}

	public bool CheckWithPointInCenter(BoardPosition point, int radius = 3)
	{
		var field = new List<BoardPosition>();
		return FindNearWithPointInCenter(point, field, 1, radius);
	}
	
	public List<BoardPosition> FindNearWithPointInCenter(BoardPosition point, int amount, int radius = 3)
	{
		var result = new List<BoardPosition>();
		
		FindNearWithPointInCenter(point, result, amount, radius);
		
		return result;
	}
	
	public bool FindNearWithPointInCenter(BoardPosition point, List<BoardPosition> field, int amount, int radius = 3)
	{
		for (var i = 0; i < radius; i++)
		{
			FindRingWithPointInCenter(point, field, amount, i);
				
			if (field.Count >= amount) return true;
		}
		
		return field.Count != 0;
	}
	
	public bool FindNearRingWithPointInCenter(BoardPosition point, List<BoardPosition> field, int radius = 3)
	{
		for (var i = 1; i < radius; i++)
		{
			FindRingWithPointInCenter(point, field, (i*2)*4, i);
		}
		
		return field.Count != 0;
	}
	
	public bool FindRingWithPointInCenter(BoardPosition point, List<BoardPosition> field, int count, int radius, bool checkCount = false)
	{
		// TODO: нет проверки на валидность координаты, в результате идет проход даже по ячекам, которые не существуют
		
		radius = Mathf.Max(radius, 0);
		
		var isStartAdded = AddIsEmpty(point, field, count);
		
		if (radius == 0) return isStartAdded;
		if (isStartAdded && checkCount) return true;
		
		var bottomLeft = point.BottomLeftAtDistance(radius);
		var topLeft = point.TopLeftAtDistance(radius);
		var topRight = point.TopRightAtDistance(radius);
		var bottomRight = point.BottomRightAtDistance(radius);
			
		for (var j = radius * 2 - 1; j >= 0; j--)
		{
			bottomLeft = bottomLeft.Up;
				
			if (AddIsEmpty(bottomLeft, field, count) && checkCount) return true;
				
			topLeft = topLeft.Right;
				
			if (AddIsEmpty(topLeft, field, count) && checkCount) return true;
				
			topRight = topRight.Down;
				
			if (AddIsEmpty(topRight, field, count) && checkCount) return true;
				
			bottomRight = bottomRight.Left;
				
			if (AddIsEmpty(bottomRight, field, count) && checkCount) return true;
		}

		return field.Count != 0;
	}

	public List<BoardPosition> FindEmptyAreaByPoint(BoardPosition targetPoint)
	{
		var result = FindAreaByPoint(targetPoint, (pos, controller) => controller.BoardLogic.GetPieceAt(pos) == null);
		return result;
	}
	
	public List<BoardPosition> FindAreaByPoint(BoardPosition targetPoint, Func<BoardPosition, BoardController, bool> condition)
	{   
        var checkedPositions = new HashSet<BoardPosition>();
        var uncheckedPositions = new HashSet<BoardPosition>();

		var resultCollection = new List<BoardPosition>();

        uncheckedPositions.Add(targetPoint);
		
        while (uncheckedPositions.Count > 0)
        {
            var current = uncheckedPositions.Last();

            uncheckedPositions.Remove(current);
            checkedPositions.Add(current);

	        if (condition(current, context.Context) && context.IsPointValid(current))
		        resultCollection.Add(current);
	        else
		        continue;
	        
            var availiablePositions = current.Neighbors();
            
	        foreach (var pos in availiablePositions)
	        {
		        if (checkedPositions.Contains(pos) == false && uncheckedPositions.Contains(pos) == false)
			        uncheckedPositions.Add(pos);
	        }
        }

		return resultCollection;
	}
	
	public bool CheckFreeSpaceNearPosition(BoardPosition position, int amount)
	{
		if (amount == 0) return true;
		
		var free = new List<BoardPosition>();
		if (!FindRandomNearWithPointInCenter(position, free, amount))
			return false;
		
		return free.Count >= amount;
	}
	
	public bool CheckFreeSpaceReward(int amount, out BoardPosition target)
	{
		var board = BoardService.Current.FirstBoard;
		var positions = board.BoardLogic.PositionsCache.GetRandomPositions(PieceTypeFilter.Character, 1);
		
		target = BoardPosition.Default();

		if (amount == 0 && positions.Count != 0)
		{
			target = positions[0];
			return true;
		}

		foreach (var position in positions)
		{
			if (CheckFreeSpaceNearPosition(position, amount) == false) continue;
			
			target = position;
			return true;
		}
		
		return false;
	}
	
	public bool FindAllWithPointInCenter(BoardPosition point, int width, int height, List<BoardPosition> field, int targetLayer)
	{
		var startPoint = CenterToHome(point, width, height);
		
		return FindAllWithPointInHome(startPoint, width, height, field, targetLayer);
	}
    
    public List<BoardPosition> FindAllWithCondition(Func<BoardPosition, bool> condition, int targetLayer)
    {
        int xMin;
        int xMax;
		
        int yMin;
        int yMax;

        context.IsXValid(0,                    out xMin);
        context.IsXValid(context.CurrentWidth, out xMax);
		
        context.IsYValid(0,                     out yMin);
        context.IsYValid(context.CurrentHeight, out yMax);
        
        var field = new List<BoardPosition>();
		
        for (var i = xMin; i <= xMax; i++)
        {
            for (var j = yMin; j <= yMax; j++)
            {
                var empty = new BoardPosition(i, j, targetLayer);

                if (condition.Invoke(empty) == false) continue;
				
                field.Add(empty);
            }
        }
		
        return field;
    }
    
    public List<BoardPosition> FindAll()
    {
        int xMin;
        int xMax;
		
        int yMin;
        int yMax;

        context.IsXValid(0,             out xMin);
        context.IsXValid(context.CurrentWidth, out xMax);
		
        context.IsYValid(0,              out yMin);
        context.IsYValid(context.CurrentHeight, out yMax);
        
        var field = new List<BoardPosition>();
		
        for (var i = xMin; i <= xMax; i++)
        {
            for (var j = yMin; j <= yMax; j++)
            {
                var empty = new BoardPosition(i, j);

                if (context.IsEmpty(empty) == false) continue;
				
                field.Add(empty);
            }
        }
		
        return field;
    }

	public bool FindAllWithPointInHome(BoardPosition point, int width, int height, List<BoardPosition> field, int targetLayer)
	{
		int xMin;
		int xMax;
		
		int yMin;
		int yMax;

		context.IsXValid(point.X, out xMin);
		context.IsXValid(point.X + width - 1, out xMax);
		
		context.IsYValid(point.Y, out yMin);
		context.IsYValid(point.Y + height - 1, out yMax);
		
		for (var i = xMin; i <= xMax; i++)
		{
			for (var j = yMin; j <= yMax; j++)
			{
				var empty = new BoardPosition(i, j, targetLayer);

				if (context.IsEmpty(empty) == false) continue;
				
				field.Add(empty);
			}
		}
		
		return field.Count != 0;
	}

	private BoardPosition CenterToHome(BoardPosition point, int width, int height)
	{
		var w = (width - 1) / 2;
		var h = (height - 1) / 2;
		
		return new BoardPosition(point.X - w, point.Y - h);
	}

	private bool AddIsEmpty(BoardPosition point, List<BoardPosition> field, int count)
	{
		if (point.IsValidFor(context.Context.BoardDef.Width, context.Context.BoardDef.Height)
		    && context.IsLockedCell(point) == false
		    && context.IsEmpty(point)
		    && field.IndexOf(point) == -1)
		{
			field.Add(point);
		}
		
		return field.Count == count;
	}
}