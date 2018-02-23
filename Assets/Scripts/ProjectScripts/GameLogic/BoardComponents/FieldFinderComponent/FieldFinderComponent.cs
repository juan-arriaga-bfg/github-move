using System.Collections.Generic;

public class FieldFinderComponent : ECSEntity
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();

	public override int Guid { get { return ComponentGuid; } }
	
	public bool Find(BoardController boardController, BoardPosition point, List<BoardPosition> field, out int current)
	{
		var logic = boardController.BoardLogic;
		
		current = PieceType.None.Id;
		
		if (logic.IsLockedCell(point)) return false;
		
		var piece = logic.GetPieceAt(point);
		
		if (piece == null) return false;

		current = piece.PieceType;
		
		field = FindField(boardController, piece.PieceType, point, field);
		
		return true;
	}
	
	private List<BoardPosition> FindField(BoardController boardController, int type, BoardPosition point, List<BoardPosition> field)
	{
		if(field.Contains(point) || boardController.BoardLogic.IsLockedCell(point)) return field;
		
		field.Add(point);
		
		if (PieceIsCorrect(boardController, type, point.Left))
		{
			FindField(boardController, type, point.Left, field);
		}
		
		if (PieceIsCorrect(boardController, type, point.Right))
		{
			FindField(boardController, type, point.Right, field);
		}
		
		if (PieceIsCorrect(boardController, type, point.Up))
		{
			FindField(boardController, type, point.Up, field);
		}
		
		if (PieceIsCorrect(boardController, type, point.Down))
		{
			FindField(boardController, type, point.Down, field);
		}
		
		return field;
	}

	private bool PieceIsCorrect(BoardController gameBoardController, int type, BoardPosition point)
	{
		var piece = gameBoardController.BoardLogic.GetPieceAt(point);
		
		return piece != null && piece.PieceType == type;
	}
}