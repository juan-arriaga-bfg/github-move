using System.Collections.Generic;

public class TouchReactionDefinitionOpenChestRewardWindow : TouchReactionDefinitionComponent
{
	private ChestPieceComponent chestComponent;
	private bool isComplete;
	
	public override bool Make(BoardPosition position, Piece piece)
	{
		if (isComplete) return false;
		
		chestComponent = piece.GetComponent<ChestPieceComponent>(ChestPieceComponent.ComponentGuid);
		
		if (chestComponent == null || chestComponent.Chest == null) return false;

		switch (chestComponent.Chest.State)
		{
			case ChestState.Close:
			case ChestState.InProgress:
				var model = UIService.Get.GetCachedModel<UIChestMessageWindowModel>(UIWindowType.ChestMessage);

				model.Chest = chestComponent.Chest;
				model.OnStart = () => Start(piece);
				model.OnBoost = () => Boost(piece);
				
				UIService.Get.ShowWindow(UIWindowType.ChestMessage);
				
				break;
			case ChestState.Open:
				Open(position, piece);
				break;
			default:
				return false;
		}
		
		return true;
	}

	private void Start(Piece piece)
	{
		chestComponent.Timer.Start();
		chestComponent.Chest.State = ChestState.InProgress;
		GameDataService.Current.ChestsManager.ActiveChest = chestComponent.Chest;
		piece.ActorView.UpdateView();
	}
	
	private void Boost(Piece piece)
	{
		chestComponent.Chest.State = ChestState.Open;
		
		if(GameDataService.Current.ChestsManager.ActiveChest == chestComponent.Chest)
		{
			GameDataService.Current.ChestsManager.ActiveChest = null;
		}
		
		piece.ActorView.UpdateView();
	}

	private void Open(BoardPosition position, Piece piece)
	{
		var level = ProfileService.Current.GetStorageItem(Currency.LevelCastle.Name).Amount;
		isComplete = true;

		piece.Context.ActionExecutor.AddAction(new ChestRewardAction
		{
			From = position,
			Pieces = chestComponent.Chest.GetRewardPieces(level),
			Chargers = chestComponent.Chest.GetRewardChargers(level),
			OnComplete = new CollapsePieceToAction
			{
				To = position,
				Positions = new List<BoardPosition> {position}
			}
		});
	}
}