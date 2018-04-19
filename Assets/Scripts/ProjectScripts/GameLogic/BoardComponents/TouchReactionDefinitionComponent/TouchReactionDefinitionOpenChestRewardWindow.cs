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
				Start(piece);
				break;
			case ChestState.InProgress:
				Boost(piece);
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
		if (GameDataService.Current.ChestsManager.ActiveChest != null)
		{
			UIMessageWindowController.CreateDefaultMessage("Another chest in the process!");
			return;
		}
		
		UIMessageWindowController.CreateDefaultMessage("Start Chest?", () =>
		{
			chestComponent.Timer.Start();
			chestComponent.Chest.State = ChestState.InProgress;
			GameDataService.Current.ChestsManager.ActiveChest = chestComponent.Chest;
			piece.ActorView.UpdateView();
		});
	}
	
	private void Boost(Piece piece)
	{
		UIMessageWindowController.CreateDefaultMessage("Open Chest?", () =>
		{
			chestComponent.Chest.State = ChestState.Open;
			GameDataService.Current.ChestsManager.ActiveChest = null;
			piece.ActorView.UpdateView();
		});
	}

	private void Open(BoardPosition position, Piece piece)
	{
		var level = ProfileService.Current.GetStorageItem(Currency.LevelCastle.Name).Amount + 1;
		isComplete = true;

		piece.Context.ActionExecutor.AddAction(new ChestRewardAction
		{
			From = position,
			Rewards = chestComponent.Chest.GetRewards(level),
			OnComplete = new CollapsePieceToAction
			{
				To = position,
				Positions = new List<BoardPosition> {position}
			}
		});
	}
	
	/*public override bool Make(BoardPosition position, Piece piece)
	{
		var chest = GameDataService.Current.ChestsManager.PieceToChest(piece.PieceType);

		if (chest == ChestType.None) return false;
		
		piece.Context.ActionExecutor.AddAction(new CollapsePieceToAction
		{
			To = position,
			Positions = new List<BoardPosition> {position},
			OnComplete = () =>
			{
				GameDataService.Current.ChestsManager.AddActiveChest(chest);
				var main = UIService.Get.GetShowedWindowByName(UIWindowType.MainWindow).CurrentView as UIMainWindowView;
				
				main.UpdateSlots();
			}
		});
		
		return true;
	}*/
	
	/*public override bool Make(BoardPosition position, Piece piece)
	{
		var chest = GameDataService.Current.ChestsManager.GetChest(piece.PieceType);

		if (chest == null) return false;
		
		piece.Context.ActionExecutor.AddAction(new CollapsePieceToAction
		{
			To = position,
			Positions = new List<BoardPosition> {position},
			OnComplete = () =>
			{
				var model = UIService.Get.GetCachedModel<UIChestRewardWindowModel>(UIWindowType.ChestRewardWindow);

				model.Chest = chest;
				UIService.Get.ShowWindow(UIWindowType.ChestRewardWindow);
			}
		});
		return true;
	}*/
}