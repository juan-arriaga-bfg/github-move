﻿public class GenericPieceBuilder : IPieceBuilder
{
    public virtual Piece Build(int pieceType, BoardController context)
    {
        var piece = new Piece(pieceType, context);

        piece.RegisterComponent(new LayerPieceComponent {Index = context.BoardDef.PieceLayer});
        
        piece.RegisterComponent(new PieceBoardObserversComponent());
        piece.RegisterComponent(new CachedPiecePositionComponent());

        return piece;
    }
}
