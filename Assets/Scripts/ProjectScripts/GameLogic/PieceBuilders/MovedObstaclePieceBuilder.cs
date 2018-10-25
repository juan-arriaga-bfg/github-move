using UnityEngine;

public class MovedObstaclePieceBuilder : GenericPieceBuilder
{
    public override Piece Build(int pieceType, BoardController context)
    {
        var piece = base.Build(pieceType, context);
		
        piece.RegisterComponent(new DraggablePieceComponent());
        
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
		
        var life = new ObstacleLifeComponent();

        piece.RegisterComponent(life);
        AddObserver(piece, life);

	    piece.RegisterComponent(new TouchReactionComponent()
		    .RegisterComponent(new TouchReactionDefinitionMenu {MainReactionIndex = 0}
			    .RegisterDefinition(new TouchReactionDefinitionOpenBubble {ViewId = ViewType.ObstacleState})
			    .RegisterDefinition(new TouchReactionDefinitionObstacleComponent {IsAutoStart = false}))
		    .RegisterComponent(new TouchReactionConditionStorage()));
		
        var pathfindLockObserver = new PathfindLockObserver() {AutoLock = true}; 
        AddObserver(piece, pathfindLockObserver);
        piece.RegisterComponent(pathfindLockObserver);
		
        return piece;
    }
}