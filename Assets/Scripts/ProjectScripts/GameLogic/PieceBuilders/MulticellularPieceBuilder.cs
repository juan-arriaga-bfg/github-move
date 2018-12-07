using System.Collections.Generic;

public class MulticellularPieceBuilder : GenericPieceBuilder 
{
	public List<BoardPosition> Mask;

	public override Piece Build(int pieceType, BoardController context)
	{
		var piece = base.Build(pieceType, context);
		
		AddObserver(piece, CreateMultiObserver());
		
		return piece;
	}
	
	protected virtual MulticellularPieceBoardObserver CreateMultiObserver()
	{
		return new MulticellularPieceBoardObserver {Mask = Mask};
	}
}