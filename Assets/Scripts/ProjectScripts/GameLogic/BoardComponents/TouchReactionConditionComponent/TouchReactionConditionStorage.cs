using UnityEngine;

public class TouchReactionConditionStorage : TouchReactionConditionComponent
{
	public override bool Check(BoardPosition position, Piece piece)
	{	
		var pathfinder = piece.GetComponent<PathfinderComponent>(PathfinderComponent.ComponentGuid);
		
		if (pathfinder == null)
		{
			Debug.LogWarning("[TouchReactionConditionStorage] PathfindComponent not found");
		}
		else
		{
			if (!pathfinder.CanPathToCastle(piece))
			{
				UIErrorWindowController.AddError("Path not found");
				return false;
			}
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
				"Complete now ",
				storage.Timer,
				() => CurrencyHellper.Purchase(Currency.Timer.Name, 1, storage.Timer.GetPrise(), success =>
				{
					if(success == false) return;
					
					storage.Timer.Stop();
					storage.Timer.OnComplete();
					
					var life = piece.GetComponent<StorageLifeComponent>(StorageLifeComponent.ComponentGuid);
					
					piece.Context.WorkerLogic.Return(life.Key);
				}));
		}
        
		return IsDone;
	}
}