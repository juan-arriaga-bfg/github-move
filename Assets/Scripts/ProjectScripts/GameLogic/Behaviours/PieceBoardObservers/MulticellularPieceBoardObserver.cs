using System.Collections.Generic;

public class MulticellularPieceBoardObserver : IPieceBoardObserver
{
	public List<BoardPosition> Mask;
	
	public void OnAddToBoard(BoardPosition position, Piece context = null)
	{
		for (int i = 0; i < Mask.Count; i++)
		{
			var point = GetPointInMask(position, Mask[i]);
			context.Context.BoardLogic.AddPieceToBoardSilent(point.X, point.Y, context);
		}
	}

	public void OnMovedFromTo(BoardPosition @from, BoardPosition to, Piece context = null)
	{
		
	}

	public void OnRemoveFromBoard(BoardPosition position, Piece context = null)
	{
		for (int i = 0; i < Mask.Count; i++)
		{
			var point = GetPointInMask(position, Mask[i]);
		}
	}

	private BoardPosition GetPointInMask(BoardPosition position, BoardPosition mask)
	{
		return new BoardPosition(position.X + mask.X, position.Y + mask.Y, position.Z);
	}
}