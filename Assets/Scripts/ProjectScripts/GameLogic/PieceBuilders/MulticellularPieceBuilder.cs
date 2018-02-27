using System.Collections.Generic;

public class MulticellularPieceBuilder : IPieceBuilder 
{
	public List<BoardPosition> Mask;
	
	public virtual Piece Build(int pieceType, BoardController context)
	{
		var piece = new Piece(pieceType, context);

		piece.RegisterComponent(new LayerPieceComponent {Index = context.BoardDef.PieceLayer});
		
		piece.RegisterComponent(new PieceBoardObserversComponent()
			.RegisterObserver(new MulticellularPieceBoardObserver {Mask = Mask}));
		
		return piece;
	}
}