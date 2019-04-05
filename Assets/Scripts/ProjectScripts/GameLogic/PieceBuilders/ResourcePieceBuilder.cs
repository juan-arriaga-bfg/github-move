public class ResourcePieceBuilder : GenericPieceBuilder
{
    public override Piece Build(int pieceType, BoardController context)
    {
        var piece = base.Build(pieceType, context);

        piece.RegisterComponent(new DraggablePieceComponent());

        var def = GameDataService.Current.PiecesManager.GetPieceDef(pieceType);

        if (def == null) return piece;

        piece.RegisterComponent(new PiecePathfindBoardCondition(piece.Context, piece)
            .RegisterComponent(PathfindIgnoreBuilder.Build(piece.PieceType)));

        piece.RegisterComponent(new ResourceStorageComponent {Resources = def.SpawnResources});

        piece.RegisterComponent(new TouchReactionComponent()
            .RegisterComponent(new TouchReactionDefinitionCollectResource())
            .RegisterComponent(new TouchReactionConditionComponent()));

        return piece;
    }
}