public class MulticellularDraggablePieceBuilder : MulticellularPieceBuilder
{
    public override Piece Build(int pieceType, BoardController context)
    {
        var piece = base.Build(pieceType, context);
        
        CreateViewComponent(piece);
        piece.RegisterComponent(new DraggablePieceComponent());
        
        return piece;
    }
}