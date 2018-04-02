using System.Collections.Generic;

public class MulticellularPieceBuilder : IPieceBuilder 
{
	public List<BoardPosition> Mask;
	
	public virtual Piece Build(int pieceType, BoardController context)
	{
		var piece = new Piece(pieceType, context);
		var view = new ViewDefinitionComponent();
		var multi = CreateMultiObserver();
        
		piece.RegisterComponent(view);
		piece.RegisterComponent(multi);
		
		piece.RegisterComponent(new LayerPieceComponent {Index = context.BoardDef.PieceLayer});
		
		piece.RegisterComponent(new PieceBoardObserversComponent()
			.RegisterObserver(view)
			.RegisterObserver(multi));
		
		piece.RegisterComponent(new CachedPiecePositionComponent());
		
		return piece;
	}
	
	protected virtual MulticellularPieceBoardObserver CreateMultiObserver()
	{
		return new MulticellularPieceBoardObserver {Mask = Mask};
	}

	protected void AddView(Piece piece, ViewType id)
	{
		var view = piece.GetComponent<ViewDefinitionComponent>(ViewDefinitionComponent.ComponentGuid);

		if (view == null) return;
		
		if(view.ViewIds == null) view.ViewIds = new List<ViewType>();
		
		view.ViewIds.Add(id);
	}
}