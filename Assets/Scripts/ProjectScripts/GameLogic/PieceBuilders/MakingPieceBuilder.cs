public class MakingPieceBuilder : MulticellularDraggablePieceBuilder
{
    public override Piece Build(int pieceType, BoardController context)
    {
        var piece = base.Build(pieceType, context);
		
        AddObserver(piece, new MakingLifeComponent());

        piece.RegisterComponent(new TouchReactionComponent()
            .RegisterComponent(new TouchReactionDefinitionMenu {MainReactionIndex = 0}
                .RegisterDefinition(new TouchReactionDefinitionOpenBubble {ViewId = ViewType.ObstacleBubble})
                .RegisterDefinition(new TouchReactionDefinitionSpawnRewards()))
            .RegisterComponent(new TouchReactionConditionWorkplace()));
		
        return piece;
    }
}