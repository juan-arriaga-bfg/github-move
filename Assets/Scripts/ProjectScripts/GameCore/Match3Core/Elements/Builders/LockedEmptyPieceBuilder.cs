public class LockedEmptyPieceBuilder : GenericPieceBuilder
{
    public override Piece Build(int pieceType, BoardController context)
    {
        var piece = base.Build(pieceType, context);
		
        CreateViewComponent(piece);

        piece.RegisterComponent(new PiecePathfindBoardCondition(context, piece)
                .RegisterComponent(PathfindIgnoreBuilder.Build(piece.PieceType)))
             .RegisterComponent(new DraggablePieceComponent());
        AddPathfindLockObserver(piece, true);
		
        return piece;
    }
}