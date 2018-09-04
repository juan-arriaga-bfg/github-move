public class PartPieceBuilder : BuildingPieceBuilder
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