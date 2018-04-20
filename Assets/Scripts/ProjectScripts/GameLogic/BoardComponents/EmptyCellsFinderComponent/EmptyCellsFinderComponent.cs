using System.Collections.Generic;
using UnityEngine;

public class EmptyCellsFinderComponent: IECSComponent
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	
	public int Guid { get { return ComponentGuid; } }
	
	private BoardLogicComponent context;
	
	public void OnRegisterEntity(ECSEntity entity)
	{
		this.context = entity as BoardLogicComponent;
	}
	
	public void OnUnRegisterEntity(ECSEntity entity)
	{
	}
	
	public bool FindRandomNearWithPointInCenter(BoardPosition point, List<BoardPosition> field, int count)
	{
		var index = 0;

		while (field.Count < count && index < 10)
		{
			index++;
			FindRingWithPointInCenter(point, field, (index * 2) * 4, index);
		}

		if (field.Count == 0)
		{
			return false;
		}

		field.Shuffle();
		
		if (field.Count > count)
		{
			field.RemoveRange(count, field.Count - count);
		}

		return field.Count != 0;
	}

	public bool CheckWithPointInCenter(BoardPosition point, int radius = 3)
	{
		var field = new List<BoardPosition>();
		return FindNearWithPointInCenter(point, field, 1, radius);
	}
	
	public bool FindNearWithPointInCenter(BoardPosition point, List<BoardPosition> field, int count, int radius = 3)
	{
		for (var i = 0; i < radius; i++)
		{
			FindRingWithPointInCenter(point, field, count, i);
				
			if (field.Count >= count) return true;
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
	
	public bool FindRingWithPointInCenter(BoardPosition point, List<BoardPosition> field, int count, int radius, bool chechCount = false)
	{
		// TODO: нет проверки на валидность координаты, в результате идет проход даже по ячекам, которые не существуют
		
		radius = Mathf.Max(radius, 0);

		if (radius == 0)
		{
			return AddIsEmpty(point, field, count);
		}
		
		var bottomLeft = point.BottomLeftAtDistance(radius);
		var topLeft = point.TopLeftAtDistance(radius);
		var topRight = point.TopRightAtDistance(radius);
		var bottomRight = point.BottomRightAtDistance(radius);
			
		for (var j = radius * 2 - 1; j >= 0; j--)
		{
			bottomLeft = bottomLeft.Up;
				
			if (AddIsEmpty(bottomLeft, field, count) && chechCount) return true;
				
			topLeft = topLeft.Right;
				
			if (AddIsEmpty(topLeft, field, count) && chechCount) return true;
				
			topRight = topRight.Down;
				
			if (AddIsEmpty(topRight, field, count) && chechCount) return true;
				
			bottomRight = bottomRight.Left;
				
			if (AddIsEmpty(bottomRight, field, count) && chechCount) return true;
		}

		return field.Count != 0;
	}
	
	public bool FindAllWithPointInCenter(BoardPosition point, int width, int height, List<BoardPosition> field)
	{
		var startPoint = CenterToHome(point, width, height);
		
		return FindAllWithPointInHome(startPoint, width, height, field);
	}

	public bool FindAllWithPointInHome(BoardPosition point, int width, int height, List<BoardPosition> field)
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
				var empty = new BoardPosition(i, j);

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
		if (point.IsValid && context.IsLockedCell(point) == false && context.IsEmpty(point)) field.Add(point);
		
		return field.Count == count;
	}
}