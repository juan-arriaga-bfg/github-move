public class PartPieceBuilder : SimplePieceBuilder
{
    public override Piece Build(int pieceType, BoardController context)
    {
        var piece = base.Build(pieceType, context);
        
        piece.Matchable?.Locker.Lock(piece);
        
        CreateViewComponent(piece);
        AddObserver(piece, new PartPieceBoardObserver());
        
        return piece;
    }
}