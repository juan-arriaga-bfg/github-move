using System.Collections.Generic;
using UnityEngine;

public class MinePieceBuilder : MulticellularPieceBuilder
{
	public override Piece Build(int pieceType, BoardController context)
	{
		var piece = base.Build(pieceType, context);
		
		CreateViewComponent(piece);
		
		piece.RegisterComponent(new DraggablePieceComponent());
		piece.RegisterComponent(new TimerComponent());
		
		var storage = new StorageComponent
		{
			IsAutoStart = false,
			IsTimerShow = true,
			TimerOffset = new Vector2(0f, 1.4f)
		};
		
		piece.RegisterComponent(storage);
		AddObserver(piece, storage);
		
		var life = new MineLifeComponent();
		
		piece.RegisterComponent(life);
		AddObserver(piece, life);
		
		piece.RegisterComponent(new TouchReactionComponent()
			 .RegisterComponent(new TouchReactionDefinitionMenu{MainReactionIndex = 0}
				 .RegisterDefinition(new TouchReactionDefinitionOpenBubble{ViewId = ViewType.ObstacleState})
				 .RegisterDefinition(new TouchReactionDefinitionObstacleComponent{IsAutoStart = false}))
			 .RegisterComponent(new TouchReactionConditionStorage()))
			 .RegisterComponent(new PiecePathfindBoardCondition(context, piece)
			 	.RegisterComponent(PathfindIgnoreBuilder.Build(piece.PieceType)));

		AddPathfindLockObserver(piece, true);
		
		return piece;
	}
}