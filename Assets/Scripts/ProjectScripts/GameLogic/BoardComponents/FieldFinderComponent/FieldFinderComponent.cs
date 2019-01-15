using System;
using System.Collections.Generic;
using UnityEngine;

public class FieldFinderComponent : IECSComponent
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	public int Guid => ComponentGuid;

	private BoardLogicComponent context;
	
	public void OnRegisterEntity(ECSEntity entity)
	{
		context = entity as BoardLogicComponent;
	}

	public void OnUnRegisterEntity(ECSEntity entity)
	{
	}
	
	public bool Find(BoardPosition point, List<BoardPosition> field, out int current, bool ignoreLock = false)
	{
		current = PieceType.None.Id;
		
		if (context.IsLockedCell(point) && !ignoreLock) return false;
		
		var piece = context.GetPieceAt(point);

		if (piece == null || PieceIsCorrect(piece.PieceType, point) == false) return false;
		
		current = piece.PieceType;
		
		field = FindField(piece.PieceType, point, field, ignoreLock);
		
		return true;
	}

	public List<BoardPosition> FindWhere(Func<BoardPosition, BoardLogicComponent, bool> predicate, bool checkLock = true)
	{
		var field = new List<BoardPosition>();
		
		var def = context.Context.BoardDef;
		for (int i = 0; i < def.Width; i++)
		{
			for (int j = 0; j < def.Height; j++)
			{
				var pos = new BoardPosition(i, j, BoardLayer.Piece.Layer);
				if((checkLock && context.IsLockedCell(pos)) || predicate(pos, context) == false)
					continue;
				field.Add(pos);
			}
		}

		return field;
	}

	private List<BoardPosition> FindField(int type, BoardPosition point, List<BoardPosition> field, bool ignoreLock = false)
	{
		if(field.Contains(point) || (context.IsLockedCell(point) && !ignoreLock)) return field;
		
		field.Add(point);
		
		if (PieceIsCorrect(type, point.Left))
		{
			FindField(type, point.Left, field);
		}
		
		if (PieceIsCorrect(type, point.Right))
		{
			FindField(type, point.Right, field);
		}
		
		if (PieceIsCorrect(type, point.Up))
		{
			FindField(type, point.Up, field);
		}
		
		if (PieceIsCorrect(type, point.Down))
		{
			FindField(type, point.Down, field);
		}
		
		return field;
	}

	private bool PieceIsCorrect(int type, BoardPosition point)
	{
		var piece = context.GetPieceAt(point);

		if (piece == null || piece.Matchable?.IsMatchable() == false) return false;
		
		return piece.PieceType == type;
	}
}