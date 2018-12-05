public class CharacterPieceBuilder : GenericPieceBuilder
{
	public override Piece Build(int pieceType, BoardController context)
	{
		var piece = base.Build(pieceType, context);
		
		CreateViewComponent(piece);
		
		AddPathfindLockObserver(piece, true);

		piece.RegisterComponent(new TimerComponent());
		
		var customer = new CustomerComponent();
		
		AddObserver(piece, customer);
		
		piece.RegisterComponent(new DraggablePieceComponent());
		
		piece.RegisterComponent(new TouchReactionComponent()
			.RegisterComponent(new TouchReactionDefinitionMenu {MainReactionIndex = 0, OnMake = success =>
					{
						var model = UIService.Get.GetCachedModel<UIOrdersWindowModel>(UIWindowType.OrdersWindow);
						model.Select = customer.Order;
						
						customer.UpdateState(OrderState.Waiting);
					}
				}
				.RegisterDefinition(new TouchReactionDefinitionOpenWindow {WindowType = UIWindowType.OrdersWindow})
				.RegisterDefinition(new TouchReactionDefinitionSpawnShop()))
			.RegisterComponent(new TouchReactionConditionCharacter()));
		
		piece.RegisterComponent(new PiecePathfindBoardCondition(context, piece)
				.RegisterComponent(PathfindIgnoreBuilder.Build(piece.PieceType)));
		
		return piece;
	}
}