public class CharacterPieceBuilder : GenericPieceBuilder
{
	public override Piece Build(int pieceType, BoardController context)
	{
		var piece = base.Build(pieceType, context);
		
		CreateViewComponent(piece);
		
		var pathfindLockObserver = new PathfindLockObserver() {AutoLock = true}; 
		
		AddObserver(piece, pathfindLockObserver);
		piece.RegisterComponent(pathfindLockObserver);
		
		piece.RegisterComponent(new DraggablePieceComponent());
		
		piece.RegisterComponent(new TouchReactionComponent()
			.RegisterComponent(new TouchReactionDefinitionMenu {MainReactionIndex = 0}
				.RegisterDefinition(new TouchReactionDefinitionOpenWindow {WindowType = UIWindowType.OrdersWindow})
				.RegisterDefinition(new TouchReactionDefinitionSpawnShop()))
			.RegisterComponent(new TouchReactionConditionComponent()));
		
		piece.RegisterComponent(new PiecePathfindBoardCondition(context, piece)
				.RegisterComponent(PathfindIgnoreBuilder.Build(piece.PieceType)));
		
		return piece;
	}
}