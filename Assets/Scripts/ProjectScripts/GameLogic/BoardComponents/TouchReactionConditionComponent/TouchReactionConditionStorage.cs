public class TouchReactionConditionStorage : TouchReactionConditionComponent
{
	public override bool Check(BoardPosition position, Piece piece)
	{	
		var storage = piece.GetComponent<StorageComponent>(StorageComponent.ComponentGuid);
		
		if (storage == null) return true;
		
		if (IsDone) return true;
        
		IsDone = !storage.Timer.IsExecuteable();
		
		if (IsDone == false)
		{
			if (storage.Timer.IsFree())
			{
				storage.Timer.FastComplete();
				return false;
			}

			if (storage.Timer.GetPrise() != null)
			{
				UIMessageWindowController.CreateTimerCompleteMessage(
					LocalizationService.Get("window.timerComplete.message.default", "window.timerComplete.message.default"),
					storage.Timer);
				
				return false;
			}
		}
        
		return IsDone;
	}
}