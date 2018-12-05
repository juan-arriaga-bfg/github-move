using System.Collections.Generic;

public class GenericPieceBuilder : IPieceBuilder
{
    public virtual Piece Build(int pieceType, BoardController context)
    {
        var piece = new Piece(pieceType, context);

        piece.RegisterComponent(new LayerPieceComponent {Index = BoardLayer.Piece.Layer});
        
        piece.RegisterComponent(new PieceBoardObserversComponent());
        piece.RegisterComponent(new CachedPiecePositionComponent());
        
        //AddPathfindLockObserver(piece, false, new List<LockerComponent>());
       
        AddMatchableComponent(piece);
        
        return piece;
    }
    
    protected ViewDefinitionComponent CreateViewComponent(Piece piece)
    {
        if (piece.ViewDefinition != null) return piece.ViewDefinition;
        
        var view = new ViewDefinitionComponent();
        
        AddObserver(piece, view);
        
        return view;
    }

    protected virtual void AddMatchableComponent(Piece piece)
    {
        piece.RegisterComponent(new MatchablePieceComponent());
    }

    protected void AddPathfindLockObserver(Piece piece, bool autoLock)
    {   
        if (piece.PathfindLockObserver != null)
        {
            var lockObserver = piece.PathfindLockObserver;
            lockObserver.AutoLock = autoLock;
            return;
        }
        
        var pathfindLockObserver = new PathfindLockObserver() {AutoLock = autoLock};
        AddObserver(piece, pathfindLockObserver);
        piece.RegisterComponent(pathfindLockObserver);
    }
    
    protected void AddObserver(Piece piece, IPieceBoardObserver observer)
    {
        var observers = piece.GetComponent<PieceBoardObserversComponent>(PieceBoardObserversComponent.ComponentGuid);
        
        piece.RegisterComponent(observer as IECSComponent);
        observers?.RegisterObserver(observer);
    }
    
    protected void AddView(Piece piece, ViewType id)
    {
        var view = piece.ViewDefinition ?? CreateViewComponent(piece);

        if(view.ViewIds == null) view.ViewIds = new List<ViewType>();
		
        view.ViewIds.Add(id);
    }
}
