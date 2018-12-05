using UnityEngine;

public class ObstaclePieceBuilder : GenericPieceBuilder
{
	public override Piece Build(int pieceType, BoardController context)
	{
		var piece = base.Build(pieceType, context);
		
		CreateViewComponent(piece);
		
		AddObserver(piece, new PathfindLockObserver {AutoLock = true});
		AddObserver(piece, new ObstacleLifeComponent());
		
		piece.RegisterComponent(new TouchReactionComponent()
			.RegisterComponent(new TouchReactionDefinitionMenu{MainReactionIndex = 0}
				.RegisterDefinition(new TouchReactionDefinitionOpenBubble{ViewId = ViewType.ObstacleBubble})
				.RegisterDefinition(new TouchReactionDefinitionSpawnRewards()))
			.RegisterComponent(new TouchReactionConditionWorkplace()))
			.RegisterComponent(new PiecePathfindBoardCondition(context, piece)
				.RegisterComponent(PathfindIgnoreBuilder.Build(piece.PieceType)));
		
		AddObserver(piece, new AreaRecalculateObserver());
		
		AddPathfindLockObserver(piece, true);
		
		return piece;
	}
}