using UnityEngine;

public class MakingPieceBuilder : MulticellularDraggablePieceBuilder
{
    public override Piece Build(int pieceType, BoardController context)
    {
        var piece = base.Build(pieceType, context);
		
        piece.RegisterComponent(new TimerComponent());
		
        var life = new MakingLifeComponent();

        piece.RegisterComponent(life);
        AddObserver(piece, life);
		
        var storage = new StorageComponent
        {
            IsAutoStart = false,
            IsTimerShow = true,
            TimerOffset = new Vector2(0f, 0.5f)
        };
		
        piece.RegisterComponent(storage);
        AddObserver(piece, storage);

        piece.RegisterComponent(new TouchReactionComponent()
            .RegisterComponent(new TouchReactionDefinitionMenu {MainReactionIndex = 0}
                .RegisterDefinition(new TouchReactionDefinitionOpenBubble {ViewId = ViewType.ObstacleState})
                .RegisterDefinition(new TouchReactionDefinitionSpawnInStorage {IsAutoStart = false}))
            .RegisterComponent(new TouchReactionConditionStorage()));
		
        return piece;
    }
}