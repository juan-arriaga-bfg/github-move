public class TavernPieceBuilder : MulticellularPieceBuilder
{
	public override Piece Build(int pieceType, BoardController context)
	{
		var piece = base.Build(pieceType, context);
		
		piece.RegisterComponent(new TouchReactionComponent()
			.RegisterComponent(new TouchReactionDefinitionMenu()
				.RegisterDefinition(new TouchReactionDefinitionOpenHeroesWindow(), "face_Robin")
				.RegisterDefinition(new TouchReactionDefinitionUpgrade(), "arrow"))
			.RegisterComponent(new TouchReactionConditionComponent()));
        
		AddView(piece, ViewType.LevelLabel);
		AddView(piece, ViewType.Menu);
		
		return piece;
	}
}