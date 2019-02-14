public class TouchReactionDefinitionBuilding : TouchReactionDefinitionComponent
{
	public override bool Make(BoardPosition position, Piece piece)
	{
		piece.Context.BoardEvents.RaiseEvent(GameEventsCodes.ClosePieceUI, position);

		if (piece.PieceState.State == BuildingState.InProgress)
		{
			UIMessageWindowController.CreateTimerCompleteMessage(
				LocalizationService.Get("window.timerComplete.message.piece", "window.timerComplete.message.piece"),
				"skip_build",
				piece.PieceState.Timer);
			
			return false;
		}
		
		piece.PieceState.OnChange();
		
		return true;
	}
}