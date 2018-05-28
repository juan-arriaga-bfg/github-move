using System.Collections.Generic;

public class TouchReactionDefinitionSpawnInCastle : TouchReactionDefinitionComponent
{
	public override bool IsViewShow(ViewDefinitionComponent viewDefinition)
	{
		return viewDefinition != null && viewDefinition.AddView(ViewType.StorageState).IsShow;
	}
    
	public override bool Make(BoardPosition position, Piece piece)
	{
		var storage = piece.GetComponent<StorageComponent>(StorageComponent.ComponentGuid);

		if (storage == null) return false;

		int amount;
        
		if (storage.Scatter(out amount) == false)
		{
			UIErrorWindowController.AddError("Production of the resource is not complete!");
			return false;
		}
        
		var free = new List<BoardPosition>();
		var positions = new List<BoardPosition>();

		if (piece.Context.BoardLogic.EmptyCellsFinder.FindRandomNearWithPointInCenter(position, free, amount) == false)
		{
			return false;
		}
		
		foreach (var pos in free)
		{
			positions.Add(pos);
            
			GameDataService.Current.ChestsManager.AddToBoard(pos, storage.SpawnPiece, true);
			if(positions.Count == amount) break;
		}
        
		piece.Context.ActionExecutor.AddAction(new ReproductionPieceAction
		{
			From = position,
			Piece = storage.SpawnPiece,
			Positions = positions
		});
        
		return true;
	}
}