public class ReproductionPieceBuilder : SimplePieceBuilder
{
	public override Piece Build(int pieceType, BoardController context)
	{
		var piece = base.Build(pieceType, context);
		
		CreateViewComponent(piece);
		
		AddObserver(piece, new ReproductionLifeComponent());
		
		piece.RegisterComponent(new TouchReactionComponent()
			.RegisterComponent(new TouchReactionDefinitionMenu {MainReactionIndex = 0}
				.RegisterDefinition(new TouchReactionDefinitionOpenTutorialBubble {ViewId = ViewType.ObstacleBubble})
				.RegisterDefinition(new TouchReactionDefinitionSpawnRewards()))
			.RegisterComponent(new TouchReactionConditionWorkplace()));
		
		return piece;
	}
}