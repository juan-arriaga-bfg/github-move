public class CrystalPieceBuilder : SimplePieceBuilder
{
    public override Piece Build(int pieceType, BoardController context)
    {
        var piece = base.Build(pieceType, context);
        
        CreateViewComponent(piece);
        
        return piece;
    }
}