public class ChestPieceBuilder : SimplePieceBuilder
{
	public override Piece Build(int pieceType, BoardController context)
	{
		var piece = base.Build(pieceType, context);
        
		CreateViewComponent(piece);
        
		AddObserver(piece, new ChestPieceComponent());
        
		piece.RegisterComponent(new TouchReactionComponent()
			.RegisterComponent(new TouchReactionDefinitionMenu {MainReactionIndex = 0}
				.RegisterDefinition(new TouchReactionDefinitionOpenChestWindow())
				.RegisterDefinition(new TouchReactionDefinitionSpawnRewards()))
			.RegisterComponent(new TouchReactionConditionComponent()));
		
		return piece;
	}
	
	protected override void AddMatchableComponent(Piece piece)
	{
		piece.RegisterComponent(new MatchableChestComponent());
	}
}