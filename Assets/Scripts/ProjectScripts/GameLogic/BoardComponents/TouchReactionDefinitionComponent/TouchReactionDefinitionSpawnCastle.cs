using System.Collections.Generic;

public class TouchReactionDefinitionSpawnCastle : TouchReactionDefinitionComponent
{
	public int Reward = -1;
	
	public override bool IsViewShow(ViewDefinitionComponent viewDefinition)
	{
		return false;
	}
    
	public override bool Make(BoardPosition position, Piece piece)
	{
		var storage = piece.GetComponent<StorageComponent>(StorageComponent.ComponentGuid);
		
		var amount = 1;
        
		if (storage != null && storage.SpawnPiece == Reward)
		{
			if (storage.Scatter(out amount) == false)
			{
				UIErrorWindowController.AddError("Not found free cells");
				return false;
			}
		}
        
		var free = new List<BoardPosition>();
		var positions = new List<BoardPosition>();
        
		if (piece.Context.BoardLogic.EmptyCellsFinder.FindRandomNearWithPointInCenter(position, free, amount) == false)
		{
			return false;
		}

		if (free.Count < amount)
			return false;
        
		foreach (var pos in free)
		{
			positions.Add(pos);
			if(positions.Count == amount) break;
		}
        
		piece.Context.ActionExecutor.AddAction(new ReproductionPieceAction
		{
			From = position,
			Piece = Reward,
			Positions = positions
		});
		
		Reward = -1;
		return true;
	}
}