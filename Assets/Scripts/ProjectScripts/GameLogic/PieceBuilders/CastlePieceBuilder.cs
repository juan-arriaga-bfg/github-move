public class CastlePieceBuilder : MulticellularSpawnPieceBuilder
{
    public override Piece Build(int pieceType, BoardController context)
    {
        var piece = base.Build(pieceType, context);
        
        AddView(piece, ViewType.BoardTimer);
        
        return piece;
    }
}