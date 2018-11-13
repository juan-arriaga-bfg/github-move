using UnityEngine;

public class ReproductionPieceBuilder : SimplePieceBuilder
{
	public override Piece Build(int pieceType, BoardController context)
	{
		var piece = base.Build(pieceType, context);
		
		CreateViewComponent(piece);
		
		piece.RegisterComponent(new TimerComponent());
		
		var storage = new StorageComponent
		{
			IsAutoStart = false,
			IsTimerShow = true,
			TimerOffset = new Vector2(0f, 0.5f)
		};
		
		piece.RegisterComponent(storage);
		AddObserver(piece, storage);
		
		var life = new ReproductionLifeComponent();

		piece.RegisterComponent(life);
		AddObserver(piece, life);
		
		piece.RegisterComponent(new TouchReactionComponent()
			.RegisterComponent(new TouchReactionDefinitionMenu {MainReactionIndex = 0}
				.RegisterDefinition(new TouchReactionDefinitionOpenBubble {ViewId = ViewType.ObstacleState})
				.RegisterDefinition(new TouchReactionDefinitionSpawnInStorage {IsAutoStart = false}))
			.RegisterComponent(new TouchReactionConditionPR()));
		
		return piece;
	}
}