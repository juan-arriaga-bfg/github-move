public class ObstaclePieceBuilder : GenericPieceBuilder
{
	public override Piece Build(int pieceType, BoardController context)
	{
		var piece = base.Build(pieceType, context);
		
		CreateViewComponent(piece);
		
		piece.RegisterComponent(new TimerComponent());
		
		var life = new ObstacleLifeComponent();

		piece.RegisterComponent(life);
		AddObserver(piece, life);
		
		var storage = new StorageComponent{IsAutoStart = false};
		
		piece.RegisterComponent(storage);
		AddObserver(piece, storage);
		
		piece.RegisterComponent(new TouchReactionComponent()
			.RegisterComponent(new TouchReactionDefinitionMenu{MainReactionIndex = 0}
				.RegisterDefinition(new TouchReactionDefinitionOpenBubble{ViewId = ViewType.ObstacleState})
				.RegisterDefinition(new TouchReactionDefinitionSpawnInStorage{IsAutoStart = false}))
			.RegisterComponent(new TouchReactionConditionComponent()));
		
		return piece;
	}
}