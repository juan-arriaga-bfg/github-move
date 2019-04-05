public class SimplePieceBuilder : GenericPieceBuilder 
{
    public override Piece Build(int pieceType, BoardController context)
    {
        var piece = base.Build(pieceType, context);

        piece.RegisterComponent(new DraggablePieceComponent());
        piece.RegisterComponent(new PiecePathfindBoardCondition(piece.Context, piece)
                .RegisterComponent(PathfindIgnoreBuilder.Build(piece.PieceType)));
        
        return piece;
    }
}