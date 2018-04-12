using System.Collections.Generic;

public class TouchReactionDefinitionOpenChestRewardWindow : TouchReactionDefinitionComponent
{
	private bool isComplete;
	
	public override bool Make(BoardPosition position, Piece piece)
	{
		if (isComplete) return false;
		
		var chest = GameDataService.Current.ChestsManager.GetChest(piece.PieceType);

		if (chest == null) return false;

		var level = ProfileService.Current.GetStorageItem(Currency.LevelCastle.Name).Amount + 1;
		isComplete = true;

		piece.Context.ActionExecutor.AddAction(new ChestRewardAction
		{
			From = position,
			Rewards = chest.GetRewards(level),
			OnComplete = new CollapsePieceToAction
			{
				To = position,
				Positions = new List<BoardPosition> {position}
			}
		});
		
		return true;
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