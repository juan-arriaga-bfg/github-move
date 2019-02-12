using System.Collections.Generic;
using UnityEngine;

public class PiecePositionsCacheComponent : IECSComponent
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	public int Guid => ComponentGuid;
	
	private Dictionary<int, List<BoardPosition>> cache = new Dictionary<int, List<BoardPosition>>();
	public Dictionary<int, List<BoardPosition>> Cache => cache;

	private BoardLogicComponent context;
	
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

		return cache.TryGetValue(pieceType, out list) == false ? 0 : list.Count;
	}

	public List<BoardPosition> GetNearestByType(int pieceType, BoardPosition from, int amount = 1)
	{
		var list = GetPiecePositionsByType(pieceType);
		
		return list.Count == 0 ? null : from.GetImmediate(list, null, amount);
	}
	
	public List<BoardPosition> GetNearestByFilter(PieceTypeFilter filter, BoardPosition from, int amount = 1)
	{
		var list = GetPiecePositionsByFilter(filter);
		
		return list.Count == 0 ? null : from.GetImmediate(list, null, amount);
	}
	
	public List<BoardPosition> GetPiecePositionsByType(int pieceType)
	{
		List<BoardPosition> list;

		return cache.TryGetValue(pieceType, out list) == false ? new List<BoardPosition>() : new List<BoardPosition>(list);
	}

	public List<BoardPosition> GetUnlockedPiecePositionsByType(int pieceType)
	{
		var piecePositions = GetPiecePositionsByType(pieceType);
		int i = 0;
		while (i < piecePositions.Count)
		{
			if (context.Context.PathfindLocker.HasPath(context.GetPieceAt(piecePositions[i])))
				i++;
			else
				piecePositions.RemoveAt(i);
		}

		return piecePositions;
	}
	
	public List<BoardPosition> GetPiecePositionsByFilter(PieceTypeFilter filter)
	{
		var ids = PieceType.GetIdsByFilter(filter);
		var list = new List<BoardPosition>();

	    for (var i = 0; i < ids.Count; i++)
	    {
	        var id = ids[i];
	        list.AddRange(GetPiecePositionsByType(id));
	    }

	    return list;
	}
    
    public List<BoardPosition> GetPiecePositionsByFilter(PieceTypeFilter filter, PieceTypeFilter exclude)
    {
        var ids = PieceType.GetIdsByFilter(filter, exclude);
        var list = new List<BoardPosition>();

        for (var i = 0; i < ids.Count; i++)
        {
            var id = ids[i];
            list.AddRange(GetPiecePositionsByType(id));
        }

        return list;
    }

	public List<BoardPosition> GetRandomPositions(PieceTypeFilter filter, int count)
	{
		var ids = PieceType.GetIdsByFilter(filter);
		var list = new List<BoardPosition>();
		var result = new List<BoardPosition>();
		
		foreach (var id in ids)
		{
			list.AddRange(GetPiecePositionsByType(id));
		}
		
		list.Shuffle();

		count = Mathf.Min(count, list.Count);
		
		for (var i = 0; i < count; i++)
		{
			result.Add(list[i]);
		}
		
		return result;
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
		
		for (var i = 0; i < count; i++)
		{
			result.Add(list[i]);
		}
		
		return result;
	}

	public void AddPosition(int pieceType, BoardPosition position)
	{
		if (cache.TryGetValue(pieceType, out var list) == false)
		{
			list = new List<BoardPosition>();
			cache.Add(pieceType, list);
		}
		
		list.Add(position);
		context.Context.BoardEvents.RaiseEvent(GameEventsCodes.ChangePiecePosition, pieceType);
	}
	
	public void RemovePosition(int pieceType, BoardPosition position)
	{
		if (cache.TryGetValue(pieceType, out var list) == false) return;
		if (list.Remove(position)) context.Context.BoardEvents.RaiseEvent(GameEventsCodes.ChangePiecePosition, pieceType);
	}
}