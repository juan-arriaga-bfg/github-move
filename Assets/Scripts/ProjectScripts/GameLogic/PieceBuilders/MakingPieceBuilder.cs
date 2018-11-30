using UnityEngine;

public class MakingPieceBuilder : MulticellularDraggablePieceBuilder
{
    public override Piece Build(int pieceType, BoardController context)
    {
        var piece = base.Build(pieceType, context);
		
        piece.RegisterComponent(new TimerComponent());
		
        var storage = new StorageComponent
        {
            IsAutoStart = false,
            IsTimerShow = true,
            TimerOffset = new Vector2(0f, 0.5f)
        };
		
        piece.RegisterComponent(storage);
        AddObserver(piece, storage);
        
        var life = new MakingLifeComponent();

        piece.RegisterComponent(life);
        AddObserver(piece, life);

        piece.RegisterComponent(new TouchReactionComponent()
            .RegisterComponent(new TouchReactionDefinitionMenu {MainReactionIndex = 0}
                .RegisterDefinition(new TouchReactionDefinitionOpenBubble {ViewId = ViewType.ObstacleBubble})
                .RegisterDefinition(new TouchReactionDefinitionSpawnInStorage {IsAutoStart = false}))
            .RegisterComponent(new TouchReactionConditionStorage()));
		
        return piece;
    }
}