public class WorkerPieceBuilder : GenericPieceBuilder 
{
    public override Piece Build(int pieceType, BoardController context)
    {
        var piece = base.Build(pieceType, context);

        piece.RegisterComponent(new WorkerDraggablePieceComponent());
        
        piece.RegisterComponent(new PiecePathfindBoardCondition(piece.Context, piece)
                .RegisterComponent(PathfindIgnoreBuilder.Build(piece.PieceType)));
        
        AddPathfindLockObserver(piece, true);
        
        return piece;
    }
}