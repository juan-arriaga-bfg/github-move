public class TavernPieceBuilder : MulticellularPieceBuilder
{
	public override Piece Build(int pieceType, BoardController context)
	{
		var piece = base.Build(pieceType, context);
		
		piece.RegisterComponent(new TouchReactionComponent()
			.RegisterComponent(new TouchReactionDefinitionMenu()
				.RegisterDefinition(new TouchReactionDefinitionOpenHeroesWindow{Icon = "face_Robin"})
				.RegisterDefinition(new TouchReactionDefinitionUpgrade{Icon = "arrow"}))
			.RegisterComponent(new TouchReactionConditionComponent()));
		
		return piece;
	}
}