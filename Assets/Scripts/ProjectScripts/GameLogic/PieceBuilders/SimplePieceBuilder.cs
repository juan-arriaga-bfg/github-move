public class SimplePieceBuilder : GenericPieceBuilder 
{
    public override Piece Build(int pieceType, BoardController context)
    {
        var piece = base.Build(pieceType, context);

        piece.RegisterComponent(new DraggablePieceComponent());

        var def = GameDataService.Current.PiecesManager.GetPieceDef(pieceType);
        
        if (def == null) return piece;

        if (def.ReproductionDef?.Reproduction != null)
        {
            AddObserver(piece, new ReproductionPieceComponent {Child = def.ReproductionDef.Reproduction});
        }

        piece.RegisterComponent(new PiecePathfindBoardCondition(piece.Context, piece)
                .RegisterComponent(PathfindIgnoreBuilder.Build(piece.PieceType)));
        
        AddPathfindLockObserver(piece, true);
        
        return piece;
    }
}