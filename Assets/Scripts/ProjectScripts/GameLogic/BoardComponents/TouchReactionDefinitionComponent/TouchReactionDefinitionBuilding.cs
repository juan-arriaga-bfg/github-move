public class TouchReactionDefinitionBuilding : TouchReactionDefinitionComponent
{
	private ViewDefinitionComponent viewDef;
	
	public override bool Make(BoardPosition position, Piece piece)
	{
		piece.Context.BoardEvents.RaiseEvent(GameEventsCodes.ClosePieceUI, position);

		if (piece.PieceState.State == BuildingState.InProgress)
		{
			UIMessageWindowController.CreateTimerCompleteMessage(
				"Complete now!",
				"Would you like to build the item right now for crystals?",
				"Complete now ",
				piece.PieceState.Timer,
				() => CurrencyHellper.Purchase(Currency.Product.Name, 1, piece.PieceState.Timer.GetPrise(), success =>
				{
					if(success == false) return;
					piece.PieceState.Fast();
				}));
			return false;
		}
		
		piece.PieceState.OnChange();
		
		return true;
	}
}