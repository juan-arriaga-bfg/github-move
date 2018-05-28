using System.Collections.Generic;

public class TouchReactionDefinitionSpawnCastle : TouchReactionDefinitionComponent
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
		
		var level = ProfileService.Current.GetStorageItem(Currency.LevelCastle.Name).Amount;
		var chest = GameDataService.Current.ChestsManager.GetChest(storage.SpawnPiece);
		var pieces = new Dictionary<int, int>();
		var chargers = new Dictionary<string, int>();
		
		for (var i = 0; i < amount; i++)
		{
			var pDict = chest.GetRewardPieces(level);
			var cDict = chest.GetRewardChargers(level);

			foreach (var pair in pDict)
			{
				if (pieces.ContainsKey(pair.Key))
				{
					pieces[pair.Key] += pair.Value;
					continue;
				}
				
				pieces.Add(pair.Key, pair.Value);
			}

			foreach (var pair in cDict)
			{
				if (chargers.ContainsKey(pair.Key))
				{
					chargers[pair.Key] += pair.Value;
					continue;
				}
				
				chargers.Add(pair.Key, pair.Value);
			}
		}

		piece.Context.ActionExecutor.AddAction(new ChestRewardAction
		{
			From = position,
			Pieces = pieces,
			Chargers = chargers
		});
        
		return true;
	}
}