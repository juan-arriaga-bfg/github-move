﻿using System.Collections.Generic;

public class GenericPieceBuilder : IPieceBuilder
{
    public virtual Piece Build(int pieceType, BoardController context)
    {
        var piece = new Piece(pieceType, context);

        piece.RegisterComponent(new LayerPieceComponent {Index = context.BoardDef.PieceLayer});
        
        piece.RegisterComponent(new PieceBoardObserversComponent());
        piece.RegisterComponent(new CachedPiecePositionComponent());

        AddStateComponent(piece);
        AddMatchableComponent(piece);
        
        return piece;
    }
    
    protected ViewDefinitionComponent CreateViewComponent(Piece piece)
    {
        var view = piece.GetComponent<ViewDefinitionComponent>(ViewDefinitionComponent.ComponentGuid);

        if (view != null) return view;
        
        view = new ViewDefinitionComponent();
        piece.RegisterComponent(view);
        AddObserver(piece, view);
        
        return view;
    }

    protected virtual void AddMatchableComponent(Piece piece)
    {
        piece.RegisterComponent(new MatchablePieceComponent());
    }

    private void AddStateComponent(Piece piece)
    {
        if ((piece.PieceType <= PieceType.A2.Id || piece.PieceType > PieceType.A9.Id) &&
            (piece.PieceType <= PieceType.C2.Id || piece.PieceType > PieceType.C9.Id)) return;
        
        CreateViewComponent(piece);
        piece.RegisterComponent(new PieceStateComponent());
    }
    
    protected void AddObserver(Piece piece, IPieceBoardObserver observer)
    {
        var observers = piece.GetComponent<PieceBoardObserversComponent>(PieceBoardObserversComponent.ComponentGuid);

        observers?.RegisterObserver(observer);
    }
    
    protected void AddView(Piece piece, ViewType id)
    {
        var view = piece.GetComponent<ViewDefinitionComponent>(ViewDefinitionComponent.ComponentGuid) ?? CreateViewComponent(piece);

        if(view.ViewIds == null) view.ViewIds = new List<ViewType>();
		
        view.ViewIds.Add(id);
    }
}
