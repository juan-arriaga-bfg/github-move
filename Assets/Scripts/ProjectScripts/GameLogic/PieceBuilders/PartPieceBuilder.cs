public class PartPieceBuilder : SimplePieceBuilder
{
    public override Piece Build(int pieceType, BoardController context)
    {
        var piece = base.Build(pieceType, context);
        
        piece.Matchable?.Locker.Lock(piece);
        
        AddObserver(piece, new PartPieceBoardObserver());
        CreateViewComponent(piece);
        
        return piece;
    }
}