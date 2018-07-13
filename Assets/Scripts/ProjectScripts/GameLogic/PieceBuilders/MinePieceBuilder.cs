public class MinePieceBuilder : MulticellularPieceBuilder
{
	public override Piece Build(int pieceType, BoardController context)
	{
		var piece = base.Build(pieceType, context);

		AddView(piece, ViewType.LevelLabel);
        
		piece.RegisterComponent(new TimerComponent());
		
		var life = new MineLifeComponent();

		piece.RegisterComponent(life);
		AddObserver(piece, life);
        
		var storage = new StorageComponent{IsAutoStart = false};
		
		piece.RegisterComponent(storage);
		AddObserver(piece, storage);
		
		piece.RegisterComponent(new TouchReactionComponent()
			.RegisterComponent(new TouchReactionDefinitionMenu{MainReactionIndex = 0}
				.RegisterDefinition(new TouchReactionDefinitionOpenBubble{ViewId = ViewType.MineState})
				.RegisterDefinition(new TouchReactionDefinitionSpawnInStorage{IsAutoStart = false}))
			.RegisterComponent(new TouchReactionConditionComponent()));
        
		return piece;
	}
}