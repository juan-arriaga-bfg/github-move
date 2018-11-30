public class MovedObstaclePieceBuilder : GenericPieceBuilder
{
    public override Piece Build(int pieceType, BoardController context)
    {
        var piece = base.Build(pieceType, context);
		
	    CreateViewComponent(piece);
	    
        piece.RegisterComponent(new DraggablePieceComponent());
        
	    AddObserver(piece, new PathfindLockObserver {AutoLock = true});
	    AddObserver(piece, new ObstacleLifeComponent());

	    piece.RegisterComponent(new TouchReactionComponent()
		    .RegisterComponent(new TouchReactionDefinitionMenu {MainReactionIndex = 0}
			    .RegisterDefinition(new TouchReactionDefinitionOpenBubble {ViewId = ViewType.ObstacleBubble})
			    .RegisterDefinition(new TouchReactionDefinitionSpawnRewards()))
		    .RegisterComponent(new TouchReactionConditionWorkplace()));
		
        return piece;
    }
}