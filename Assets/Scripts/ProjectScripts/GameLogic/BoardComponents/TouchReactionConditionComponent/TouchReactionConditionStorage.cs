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
			UIErrorWindowController.AddError("Work is not complete!");
		}
        
		return IsDone;
	}
}