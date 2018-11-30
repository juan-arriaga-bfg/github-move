public class ObstaclePieceBuilder : GenericPieceBuilder
{
	public override Piece Build(int pieceType, BoardController context)
	{
		var piece = base.Build(pieceType, context);
		
		CreateViewComponent(piece);
		
		var pathfindLockObserver = new PathfindLockObserver {AutoLock = true};
		
		piece.RegisterComponent(pathfindLockObserver);
		AddObserver(piece, pathfindLockObserver);
		
		var life = new ObstacleLifeComponent();
		
		piece.RegisterComponent(life);
		AddObserver(piece, life);
		
		piece.RegisterComponent(new TouchReactionComponent()
			.RegisterComponent(new TouchReactionDefinitionMenu{MainReactionIndex = 0}
				.RegisterDefinition(new TouchReactionDefinitionOpenBubble{ViewId = ViewType.ObstacleBubble})
				.RegisterDefinition(new TouchReactionDefinitionSpawnRewards()))
			.RegisterComponent(new TouchReactionConditionWorkplace()))
			.RegisterComponent(new PiecePathfindBoardCondition(context, piece)
				.RegisterComponent(PathfindIgnoreBuilder.Build(piece.PieceType)));
		
		return piece;
	}
}