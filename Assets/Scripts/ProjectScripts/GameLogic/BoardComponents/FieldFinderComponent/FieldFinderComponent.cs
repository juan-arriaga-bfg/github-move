using System.Collections.Generic;
using UnityEngine;

public class FieldFinderComponent : IECSComponent
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	
	public int Guid
	{
		get { return ComponentGuid; }
	}
	
	private BoardLogicComponent context;
	
	public void OnRegisterEntity(ECSEntity entity)
	{
		this.context = entity as BoardLogicComponent;
	}

	public void OnUnRegisterEntity(ECSEntity entity)
	{
	}
	
	public bool Find(BoardPosition point, List<BoardPosition> field, out int current)
	{
		current = PieceType.None.Id;
		
		if (context.IsLockedCell(point)) return false;
		
		var piece = context.GetPieceAt(point);

		if (piece == null) return false;
		
		current = piece.PieceType;
		
		field = FindField(piece.PieceType, point, field);
		
		return true;
	}
	
	private List<BoardPosition> FindField(int type, BoardPosition point, List<BoardPosition> field)
	{
		if(field.Contains(point) || context.IsLockedCell(point)) return field;
		
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

		if (piece == null) return false;
		
		var macheble = piece.GetComponent<MatchablePieceComponent>(MatchablePieceComponent.ComponentGuid);
		
		if (macheble == null || macheble.IsMatchable() == false) return false;
		
		return piece.PieceType == type;
	}
}