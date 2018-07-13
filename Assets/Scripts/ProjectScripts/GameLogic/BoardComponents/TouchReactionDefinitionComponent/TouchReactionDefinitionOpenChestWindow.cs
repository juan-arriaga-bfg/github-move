using System.Collections.Generic;

public class TouchReactionDefinitionOpenChestWindow : TouchReactionDefinitionComponent
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
				model.OnOpen  = () => Open(position, piece);
				
				UIService.Get.ShowWindow(UIWindowType.ChestMessage);
				
				break;
			default:
				return false;
		}
		
		return true;
	}
	
	private void Open(BoardPosition position, Piece piece)
	{
		isComplete = true;

		piece.Context.ActionExecutor.AddAction(new ChestRewardAction
		{
			From = position,
			Pieces = chestComponent.Chest.GetRewardPieces(),
			OnComplete = new CollapsePieceToAction
			{
				To = position,
				Positions = new List<BoardPosition> {position}
			}
		});
	}
}