using System.Collections.Generic;

public class GenericPieceBuilder : IPieceBuilder
{
    public virtual Piece Build(int pieceType, BoardController context)
    {
        var piece = new Piece(pieceType, context);

        piece.RegisterComponent(new LayerPieceComponent {Index = context.BoardDef.PieceLayer});
        
        piece.RegisterComponent(new PieceBoardObserversComponent());
        piece.RegisterComponent(new CachedPiecePositionComponent());
        
        AddMatchableComponent(piece);
        
        return piece;
    }
    
    protected ViewDefinitionComponent CreateViewComponent(Piece piece)
    {
        if (piece.ViewDefinition != null) return piece.ViewDefinition;
        
        var view = new ViewDefinitionComponent();
        piece.RegisterComponent(view);
        AddObserver(piece, view);
        
        return view;
    }

    protected virtual void AddMatchableComponent(Piece piece)
    {
        piece.RegisterComponent(new MatchablePieceComponent());
    }
    
    protected void AddObserver(Piece piece, IPieceBoardObserver observer)
    {
        var observers = piece.GetComponent<PieceBoardObserversComponent>(PieceBoardObserversComponent.ComponentGuid);

        observers?.RegisterObserver(observer);
    }
    
    protected void AddView(Piece piece, ViewType id)
    {
        var view = piece.ViewDefinition ?? CreateViewComponent(piece);

        if(view.ViewIds == null) view.ViewIds = new List<ViewType>();
		
        view.ViewIds.Add(id);
    }
}
