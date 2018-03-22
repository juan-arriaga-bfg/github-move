using System.Collections.Generic;

public class MulticellularPieceBoardObserver : IPieceBoardObserver
{
	public List<BoardPosition> Mask;
	private BoardPosition realPosition;
	
	public void OnAddToBoard(BoardPosition position, Piece context = null)
	{
		if(context == null) return;
		
		realPosition = position;
		
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
		if(context == null || realPosition.Equals(position) == false) return;
		
		for (int i = 0; i < Mask.Count; i++)
		{
			var point = GetPointInMask(position, Mask[i]);
			context.Context.BoardLogic.RemovePieceFromBoardSilent(point);
		}
	}

	private BoardPosition GetPointInMask(BoardPosition position, BoardPosition mask)
	{
		return new BoardPosition(position.X + mask.X, position.Y + mask.Y, position.Z);
	}
}