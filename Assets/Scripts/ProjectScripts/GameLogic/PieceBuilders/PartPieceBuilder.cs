public class PartPieceBuilder : SimplePieceBuilder
{
    public override Piece Build(int pieceType, BoardController context)
    {
        var piece = base.Build(pieceType, context);
        
        var observer = new PartPieceBoardObserver();
        
        piece.RegisterComponent(observer);
        AddObserver(piece, observer);
        
        return piece;
    }
}