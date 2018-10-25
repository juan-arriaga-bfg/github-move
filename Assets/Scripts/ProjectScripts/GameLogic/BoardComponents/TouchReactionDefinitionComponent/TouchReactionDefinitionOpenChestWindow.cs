using System.Collections.Generic;
using System.Linq;

public class TouchReactionDefinitionOpenChestWindow : TouchReactionDefinitionComponent
{
	private ChestPieceComponent chestComponent;
	private bool isComplete;
	
	public override bool Make(BoardPosition position, Piece piece)
	{
		//if (isComplete) return false;
		
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

		var rewardPieces = chestComponent.Chest.GetRewardPieces();
		piece.Context.ActionExecutor.AddAction(new ChestRewardAction
		{
			From = position,
			Pieces = rewardPieces,
			OnCompleteAction = () =>
			{
				var remaind = rewardPieces.Sum(elem => elem.Value);
				if (remaind == 0)
				{
					piece.Context.ActionExecutor.AddAction(new CollapsePieceToAction
					{
						To = position,
						Positions = new List<BoardPosition> {position}
					});
				    
				    BoardService.Current.FirstBoard.BoardEvents.RaiseEvent(GameEventsCodes.OpenChest, piece);
				}
				else
				{
					piece.ActorView.UpdateView();
					//TODO storage state
				}
			}
		});
	}
}