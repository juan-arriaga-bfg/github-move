using UnityEngine;

public class UIChestsShopElementViewController : UISimpleScrollElementViewController
{
	[IWUIBinding("#ButtonLabel")] private NSText btnLabel;
	[IWUIBinding("#BuyButton")] private UIButtonViewController button;
	
	[IWUIBinding("#TimerLabel")] private NSText labelTimer;
	[IWUIBinding("#Timer")] private GameObject timer;
	
	private bool isFree;
	private bool isClick;
	private bool isReward;

	private FreeChestLogicComponent freeChestLogic;
	private FreeChestLogicComponent FreeChestLogic => freeChestLogic ?? (freeChestLogic = BoardService.Current.GetBoardById(0).FreeChestLogic);

	public override void Init()
	{
		base.Init();
		
		var contentEntity = entity as UIChestsShopElementEntity;
		
		isClick = false;
		isReward = false;
		isFree = PieceType.CH_Free.Id == contentEntity.Chest.Piece;
		
		if (isFree)
		{
			FreeChestLogic.Timer.OnExecute += UpdateLabel;
			FreeChestLogic.Timer.OnComplete += ChengeButtons;
		}

		ChengeButtons();
		
		btnLabel.Text = isFree
			? LocalizationService.Get("common.button.claim", "common.button.claim")
			: string.Format(LocalizationService.Get("common.button.buyFor", "common.button.buyFor {0}"), contentEntity.Chest.Price.ToStringIcon());
		
		button
			.ToState(GenericButtonState.Active)
			.OnClick(OnClick);
	}
	
	public override void OnViewCloseCompleted()
	{
		var contentEntity = entity as UIChestsShopElementEntity;
		
		if(contentEntity == null) return;
		
		if (isFree)
		{
			FreeChestLogic.Timer.OnExecute -= UpdateLabel;
			FreeChestLogic.Timer.OnComplete -= ChengeButtons;
		}
		
		if (isClick == false || isReward == false) return;
		
		var board = BoardService.Current.GetBoardById(0);
		var position = board.BoardLogic.PositionsCache.GetRandomPositions(PieceType.NPC_SleepingBeauty.Id, 1)[0];
		var piece = board.BoardLogic.GetPieceAt(position);
        
		var menu = piece.TouchReaction?.GetComponent<TouchReactionDefinitionMenu>(TouchReactionDefinitionMenu.ComponentGuid);
		var spawn = menu?.GetDefinition<TouchReactionDefinitionSpawnShop>();
        
		if(spawn == null) return;

		spawn.Reward = contentEntity.Chest.Piece;
		
		if(isFree) FreeChestLogic.Timer.Start();
		
		spawn.Make(piece.CachedPosition, piece);
	}

	private void UpdateLabel()
	{
		labelTimer.Text = FreeChestLogic.Timer.CompleteTime.GetTimeLeftText();
	}

	private void ChengeButtons()
	{
		var isActive = isFree && FreeChestLogic.Timer.IsExecuteable();
		
		timer.SetActive(isActive);
		button.gameObject.SetActive(!isActive);
	}

	private void OnClick()
	{
		if(isClick) return;
		
		isClick = true;
		
		var board = BoardService.Current.GetBoardById(0);
		var pos = board.BoardLogic.PositionsCache.GetRandomPositions(PieceType.NPC_SleepingBeauty.Id, 1)[0];
		
		if(!board.BoardLogic.EmptyCellsFinder.CheckFreeSpaceNearPosition(pos, 1))
		{
			isClick = false;
			UIErrorWindowController.AddError(LocalizationService.Get("message.error.freeSpace", "message.error.freeSpace"));
			return;
		}
		
		if (isFree)
		{
			OnClickFree();
			return;
		}

		OnClickPaid();
	}
	
	private void OnClickFree()
	{
		if (FreeChestLogic.Timer.IsExecuteable())
		{
			UIErrorWindowController.AddError(LocalizationService.Get("message.error.notComplete", "message.error.notComplete"));
			return;
		}
		
		isReward = true;
		context.Controller.CloseCurrentWindow();
	}

	private void OnClickPaid()
	{
		var contentEntity = entity as UIChestsShopElementEntity;
		
		CurrencyHellper.Purchase(Currency.Chest.Name, 1, contentEntity.Chest.Price, success =>
		{
			if (success == false)
			{
				isClick = false;
				return;
			}
			
			isReward = true;
			context.Controller.CloseCurrentWindow();
		});
	}
}