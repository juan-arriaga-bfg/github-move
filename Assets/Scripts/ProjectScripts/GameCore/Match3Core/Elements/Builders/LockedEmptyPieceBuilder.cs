public class LockedEmptyPieceBuilder : GenericPieceBuilder
{
    public override Piece Build(int pieceType, BoardController context)
    {
        var piece = base.Build(pieceType, context);
		
        CreateViewComponent(piece);
		
        return piece;
    }
}