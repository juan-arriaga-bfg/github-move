using System.Collections.Generic;

public class TouchReactionDefinitionOpenChestRewardWindow : TouchReactionDefinitionComponent
{
	public override bool Make(BoardPosition position, Piece piece)
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
	}
}