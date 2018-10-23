public class TouchReactionConditionStorage : TouchReactionConditionComponent
{
	public override bool Check(BoardPosition position, Piece piece)
	{
		if (piece.Context?.Pathfinder.CanPathToCastle(piece) == false)
		{
			UIErrorWindowController.AddError("Path not found");
			return false;
		}
		
		var storage = piece.GetComponent<StorageComponent>(StorageComponent.ComponentGuid);
		
		if (storage == null) return true;
		
		if (IsDone) return true;
        
		IsDone = !storage.Timer.IsExecuteable();
		
		if (IsDone == false && storage.Timer.GetPrise() != null)
		{
			UIMessageWindowController.CreateTimerCompleteMessage(
				"Complete now!",
				"Would you like to build the item right now for crystals?",
				storage.Timer);
		}
        
		return IsDone;
	}
}