using System.Collections.Generic;

public class MulticellularPieceBuilder : GenericPieceBuilder 
{
	public List<BoardPosition> Mask;

	public override Piece Build(int pieceType, BoardController context)
	{
		var piece = base.Build(pieceType, context);
		var multi = CreateMultiObserver();
		
		piece.RegisterComponent(multi);
		AddObserver(piece, multi);
		
		return piece;
	}
	
	protected virtual MulticellularPieceBoardObserver CreateMultiObserver()
	{
		return new MulticellularPieceBoardObserver {Mask = Mask};
	}
}