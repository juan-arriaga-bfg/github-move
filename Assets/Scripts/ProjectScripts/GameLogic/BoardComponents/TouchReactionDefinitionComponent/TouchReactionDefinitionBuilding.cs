public class TouchReactionDefinitionBuilding : TouchReactionDefinitionComponent
{
	public override bool Make(BoardPosition position, Piece piece)
	{
		piece.Context.BoardEvents.RaiseEvent(GameEventsCodes.ClosePieceUI, position);

		if (piece.PieceState.State == BuildingState.InProgress)
		{
			UIMessageWindowController.CreateTimerCompleteMessage(
				"Complete now!",
				"Would you like to build the item right now for crystals?",
				"Complete now ",
				piece.PieceState.Timer);
			
			return false;
		}
		
		piece.PieceState.OnChange();
		
		return true;
	}
}