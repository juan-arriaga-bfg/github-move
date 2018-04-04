using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiecePositionsCacheComponent : IECSComponent
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();

	private Dictionary<int, List<BoardPosition>> cache = new Dictionary<int, List<BoardPosition>>();

	private BoardLogicComponent context;
	
	public int Guid
	{
		get { return ComponentGuid; }
	}
	
	public void OnRegisterEntity(ECSEntity entity)
	{
		context = entity as BoardLogicComponent;
	}

	public void OnUnRegisterEntity(ECSEntity entity)
	{
	}

	public int GetCountByType(int pieceType)
	{
		List<BoardPosition> list;

		if (cache.TryGetValue(pieceType, out list) == false) return 0;

		return list.Count;
	}

	public List<BoardPosition> GetRandomPositions(int pieceType, int count)
	{
		List<BoardPosition> list;
		var result = new List<BoardPosition>();

		if (cache.TryGetValue(pieceType, out list) == false)
		{
			list = new List<BoardPosition>();
		}
		
		list.Shuffle();

		count = Mathf.Min(count, list.Count);
		
		for (int i = 0; i < count; i++)
		{
			result.Add(list[i]);
		}
		
		return result;
	}

	public void AddPosition(int pieceType, BoardPosition position)
	{
		List<BoardPosition> list;

		if (cache.TryGetValue(pieceType, out list) == false)
		{
			list = new List<BoardPosition>();
			cache.Add(pieceType, list);
		}
		
		list.Add(position);
		context.Context.BoardEvents.RaiseEvent(GameEventsCodes.ChangePiecePosition, pieceType);
	}
	
	public bool RemovePosition(int pieceType, BoardPosition position)
	{
		List<BoardPosition> list;

		if (cache.TryGetValue(pieceType, out list) == false) return false;

		var isHappened = list.Remove(position);

		if (isHappened)
		{
			context.Context.BoardEvents.RaiseEvent(GameEventsCodes.ChangePiecePosition, pieceType);
		}
		
		return isHappened;
	}
}