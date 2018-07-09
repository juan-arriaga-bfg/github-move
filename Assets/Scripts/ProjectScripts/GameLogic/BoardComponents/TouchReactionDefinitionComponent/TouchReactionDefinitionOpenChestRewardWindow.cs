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
				var model = UIService.Get.GetCachedModel<UIOldChestMessageWindowModel>(UIWindowType.OldChestMessage);

				model.Chest = chestComponent.Chest;
				model.OnStart = Start;
				model.OnBoost = Boost;
				model.OnStop = Stop;
				
				UIService.Get.ShowWindow(UIWindowType.OldChestMessage);
				
				break;
			case ChestState.Open:
				Open(position, piece);
				break;
			default:
				return false;
		}
		
		return true;
	}

	private void Start()
	{
		chestComponent.Chest.State = ChestState.InProgress;
		chestComponent.Timer.Start();
		
		GameDataService.Current.ChestsManager.ActiveChest = chestComponent.Chest;
	}
	
	private void Boost()
	{
		chestComponent.Chest.State = ChestState.Open;
		chestComponent.Timer.Stop();
		
		if(GameDataService.Current.ChestsManager.ActiveChest == chestComponent.Chest)
		{
			GameDataService.Current.ChestsManager.ActiveChest = null;
		}
	}
	
	private void Stop()
	{
		chestComponent.Chest.State = ChestState.Close;
		chestComponent.Timer.Stop();
		
		GameDataService.Current.ChestsManager.ActiveChest = null;
	}

	private void Open(BoardPosition position, Piece piece)
	{
		isComplete = true;

		piece.Context.ActionExecutor.AddAction(new ChestRewardAction
		{
			From = position,
			Pieces = chestComponent.Chest.GetRewardPieces(),
			Chargers = chestComponent.Chest.GetRewardChargers(),
			OnComplete = new CollapsePieceToAction
			{
				To = position,
				Positions = new List<BoardPosition> {position}
			}
		});
	}
}