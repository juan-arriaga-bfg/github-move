public class SimplePieceBuilder : GenericPieceBuilder 
{
    public override Piece Build(int pieceType, BoardController context)
    {
        var piece = base.Build(pieceType, context);

        piece.RegisterComponent(new DraggablePieceComponent());
        
        return piece;
    }
}