public class EmptyPieceBuilder : IPieceBuilder
{
    public virtual Piece Build(int pieceType, BoardController context)
    {
        var piece = new Piece(pieceType, context);

        piece.RegisterComponent(new LayerPieceComponent {Index = 1});
        piece.RegisterComponent(
            new PieceBoardObserversComponent()
                .RegisterObserver(new LockCellPieceBoardObserver())
        );

        return piece;
    }
}