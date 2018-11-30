public class TouchReactionConditionWorkplace : TouchReactionConditionComponent
{
	public override bool Check(BoardPosition position, Piece piece)
	{
		if (piece.Context?.Pathfinder.CanPathToCastle(piece) == false)
		{
			UIErrorWindowController.AddError(LocalizationService.Get("message.error.pieceLock", "message.error.pieceLock"));
			return false;
		}
		
		var life = piece.GetComponent<WorkplaceLifeComponent>(WorkplaceLifeComponent.ComponentGuid);

		if (life == null || IsDone) return true;
        
		IsDone = !life.Timer.IsExecuteable();

		if (IsDone) return true;
		
		if (life.Timer.IsFree())
		{
			life.Timer.FastComplete();
			return false;
		}

		if (life.Timer.GetPrise() == null) return false;
		
		UIMessageWindowController.CreateTimerCompleteMessage(
			LocalizationService.Get("window.timerComplete.message.default", "window.timerComplete.message.default"),
			life.Timer);
		
		return false;
	}
}

public class TouchReactionConditionStorage : TouchReactionConditionComponent
{
	public override bool Check(BoardPosition position, Piece piece)
	{
		if (piece.Context?.Pathfinder.CanPathToCastle(piece) == false)
		{
			UIErrorWindowController.AddError(LocalizationService.Get("message.error.pieceLock", "message.error.pieceLock"));
			return false;
		}
		
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