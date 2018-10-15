public class CharacterPieceBuilder : GenericPieceBuilder
{
	public override Piece Build(int pieceType, BoardController context)
	{
		var piece = base.Build(pieceType, context);
		
		CreateViewComponent(piece);
		
		var pathfindLockObserver = new PathfindLockObserver() {AutoLock = true}; 
		
		AddObserver(piece, pathfindLockObserver);
		piece.RegisterComponent(pathfindLockObserver);

		var customer = new CustomerComponent();
		
		piece.RegisterComponent(new TimerComponent());
		piece.RegisterComponent(customer);
		
		piece.RegisterComponent(new DraggablePieceComponent());
		
		piece.RegisterComponent(new TouchReactionComponent()
			.RegisterComponent(new TouchReactionDefinitionMenu {MainReactionIndex = 0, OnMake = success =>
					{
						customer.UpdateState(OrderState.Waiting);
					}
				}
				.RegisterDefinition(new TouchReactionDefinitionOpenWindow {WindowType = UIWindowType.OrdersWindow})
				.RegisterDefinition(new TouchReactionDefinitionSpawnShop()))
			.RegisterComponent(new TouchReactionConditionComponent()));
		
		piece.RegisterComponent(new PiecePathfindBoardCondition(context, piece)
				.RegisterComponent(PathfindIgnoreBuilder.Build(piece.PieceType)));
		
		return piece;
	}
}