using UnityEngine.UI;

public class UIChestsShopElementViewController : UISimpleScrollElementViewController
{
	[IWUIBinding("#NameLabel")] private NSText nameLabel;
	[IWUIBinding("#ButtonLabel")] private NSText btnLabel;
	
	[IWUIBinding("#BuyButton")] private UIButtonViewController btnBuy;
	[IWUIBinding("#ButtonInfo")] private UIButtonViewController btnInfo;
	
	[IWUIBinding("#ButtonBack")] private Image btnBack;
	
	private bool isFree;
	private bool isClick;
	private bool isReward;

	public override void Init()
	{
		base.Init();
		
		var contentEntity = entity as UIChestsShopElementEntity;
		
		isClick = false;
		isReward = false;
		isFree = PieceType.CH_Free.Id == contentEntity.Chest.Piece;

		nameLabel.Text = contentEntity.Name;
		
		ChengeButtons();
		
		btnLabel.Text = isFree
			? LocalizationService.Get("common.button.claim", "common.button.claim")
			: string.Format(LocalizationService.Get("common.button.buyFor", "common.button.buyFor {0}"), contentEntity.Chest.Price.ToStringIcon());
	}

	public override void OnViewShowCompleted()
	{
		base.OnViewShowCompleted();
		
		btnBuy
			.ToState(GenericButtonState.Active)
			.OnClick(OnClick);
		
		btnInfo
			.ToState(GenericButtonState.Active)
			.OnClick(OnClickInfo);
	}

	public override void OnViewCloseCompleted()
	{
		var contentEntity = entity as UIChestsShopElementEntity;
		
		if(contentEntity == null) return;
		
		if (isClick == false || isReward == false) return;
		
		var board = BoardService.Current.FirstBoard;
		var position = board.BoardLogic.PositionsCache.GetRandomPositions(PieceType.NPC_SleepingBeauty.Id, 1)[0];
		var piece = board.BoardLogic.GetPieceAt(position);
        
		var menu = piece.TouchReaction?.GetComponent<TouchReactionDefinitionMenu>(TouchReactionDefinitionMenu.ComponentGuid);
		var spawn = menu?.GetDefinition<TouchReactionDefinitionSpawnShop>();
        
		if(spawn == null) return;

		spawn.Reward = contentEntity.Chest.Piece;
		spawn.Make(piece.CachedPosition, piece);
	}
	
	private void ChengeButtons()
	{
		btnBack.sprite = IconService.Current.GetSpriteById($"button{(isFree ? "Green" : "Blue")}");
	}

	private void OnClickInfo()
	{
		UIMessageWindowController.CreateNotImplementedMessage();
	}

	private void OnClick()
	{
		if(isClick) return;
		
		isClick = true;
		
		var board = BoardService.Current.FirstBoard;
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