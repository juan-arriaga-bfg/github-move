public class ResourcePieceBuilder : SimplePieceBuilder
{
    public override Piece Build(int pieceType, BoardController context)
    {
        var piece = base.Build(pieceType, context);

        var def = GameDataService.Current.PiecesManager.GetPieceDef(pieceType);

        if (def == null) return piece;
        
        piece.RegisterComponent(new ResourceStorageComponent {Resources = def.SpawnResources});

        piece.RegisterComponent(new TouchReactionComponent()
            .RegisterComponent(new TouchReactionDefinitionCollectResource())
            .RegisterComponent(new TouchReactionConditionComponent()));
        
        return piece;
    }
}