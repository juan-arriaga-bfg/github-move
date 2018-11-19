public class TouchReactionConditionStorage : TouchReactionConditionComponent
{
	public override bool Check(BoardPosition position, Piece piece)
	{	
		var storage = piece.GetComponent<StorageComponent>(StorageComponent.ComponentGuid);
		
		if (storage == null) return true;
		
		if (IsDone) return true;
        
		IsDone = !storage.Timer.IsExecuteable();
		
		if (IsDone == false && storage.Timer.GetPrise() != null)
		{
			UIMessageWindowController.CreateTimerCompleteMessage(
				LocalizationService.Get("window.timerComplete.message.default", "Would you like to build the item right now for crystals?"),
				storage.Timer);
		}
        
		return IsDone;
	}
}