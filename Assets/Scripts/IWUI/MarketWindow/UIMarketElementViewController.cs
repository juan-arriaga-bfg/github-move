using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMarketElementViewController : UISimpleScrollElementViewController
{
	[IWUIBinding("#NameLabel")] private NSText nameLabel;
	[IWUIBinding("#ButtonLabel")] private NSText btnLabel;
	[IWUIBinding("#LockLabel")] private NSText lockAmountLabel;
	[IWUIBinding("#LockMessage")] private NSText lockMessage;
	
	[IWUIBinding("#BuyButton")] private UIButtonViewController btnBuy;
	[IWUIBinding("#ButtonInfo")] private UIButtonViewController btnInfo;
	
	[IWUIBinding("#ButtonBack")] private Image btnBack;
	
	[IWUIBinding("#LockAnchor")] private Transform lockAnchor;
	
	[IWUIBinding("#Unlock")] private GameObject unlockObj;
	[IWUIBinding("#Lock")] private GameObject lockObj;
	
	private bool isClick;
	private bool isReward;

	public override void Init()
	{
		base.Init();
		
		var contentEntity = entity as UIMarketElementEntity;
		
		isClick = false;
		isReward = false;

		var isLock = contentEntity.Def.State == MarketItemState.Lock || contentEntity.Def.State == MarketItemState.Claimed;
		
		unlockObj.SetActive(!isLock);
		lockObj.SetActive(isLock);

		nameLabel.Text = contentEntity.Name;
		lockAmountLabel.Text = contentEntity.LabelText;
		
		if (isLock) CreateIcon(lockAnchor, contentEntity.ContentId);
		
		ChengeButtons();
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
		var contentEntity = entity as UIMarketElementEntity;
		
		if(contentEntity == null) return;
		
		if (isClick == false || isReward == false) return;
		
		var board = BoardService.Current.FirstBoard;
		var position = board.BoardLogic.PositionsCache.GetRandomPositions(PieceType.NPC_SleepingBeauty.Id, 1)[0];
		
		List<CurrencyPair> currencysReward;
		var piecesReward = CurrencyHellper.FiltrationRewards(new List<CurrencyPair>{contentEntity.Def.Reward}, out currencysReward);

		contentEntity.Def.State = MarketItemState.Claimed;
		
		board.ActionExecutor.AddAction(new EjectionPieceAction
		{
			GetFrom = () => position,
			Pieces = piecesReward,
			OnComplete = () =>
			{
				var view = board.RendererContext.GetElementAt(position) as CharacterPieceView;
                
				if(view != null) view.StartRewardAnimation();
                    
				AddResourceView.Show(position, currencysReward);
			}
		});
	}
	
	private void ChengeButtons()
	{
		var contentEntity = entity as UIMarketElementEntity;
		
		btnBack.sprite = IconService.Current.GetSpriteById($"button{(contentEntity.Def.State == MarketItemState.Purchased ? "Green" : "Blue")}");
		
		btnLabel.Text = contentEntity.Def.State == MarketItemState.Purchased || contentEntity.Def.Current == null
			? LocalizationService.Get("common.button.claim", "common.button.claim")
			: string.Format(LocalizationService.Get("common.button.buyFor", "common.button.buyFor {0}"), contentEntity.Def.Current.Price.ToStringIcon());
	}

	private void OnClickInfo()
	{
		UIMessageWindowController.CreateNotImplementedMessage();
	}

	private void OnClick()
	{
		if(isClick) return;
		
		isClick = true;
		
		var contentEntity = entity as UIMarketElementEntity;
		
		if (contentEntity.Def.State == MarketItemState.Purchased)
		{
			AddReward();
			return;
		}
		
		OnClickPaid();
	}
	
	private void OnClickPaid()
	{
		var contentEntity = entity as UIMarketElementEntity;
		var price = contentEntity.Def.Current.Price;

		if (CurrencyHellper.IsCanPurchase(price, true) == false)
		{
			isClick = false;
			return;
		}
		
		if (price.Currency != Currency.Crystals.Name)
		{
			Paid();
			return;
		}

		UIMessageWindowController.CreateMessage(
			LocalizationService.Get("window.confirmation.market.title", "window.confirmation.market.title"),
			string.Format(LocalizationService.Get("window.confirmation.market.message", "window.confirmation.market.message {0}"), price.ToStringIcon()),
			contentEntity.Def.Reward.Currency,
			Paid,
			null,
			() => { isClick = false; }
		);
	}
	
	private void Paid()
	{
		var contentEntity = entity as UIMarketElementEntity;
		
		CurrencyHellper.Purchase(Currency.Chest.Name, 1, contentEntity.Def.Current.Price, success =>
		{
			if (success == false)
			{
				isClick = false;
				return;
			}
			
			contentEntity.Def.State = MarketItemState.Purchased;
			AddReward();
		});
	}

	private void AddReward()
	{
		var contentEntity = entity as UIMarketElementEntity;
		
		var board = BoardService.Current.FirstBoard;
		var pos = board.BoardLogic.PositionsCache.GetRandomPositions(PieceType.NPC_SleepingBeauty.Id, 1)[0];
		
		if(!board.BoardLogic.EmptyCellsFinder.CheckFreeSpaceNearPosition(pos, contentEntity.Def.Reward.Amount))
		{
			isClick = false;

			ChengeButtons();
			
			// No free space
			UIMessageWindowController.CreateMessage(
				LocalizationService.Get("window.daily.error.title", "window.daily.error.title"),
				LocalizationService.Get("window.daily.error.free.space", "window.daily.error.free.space"));
			
			return;
		}
		
		isReward = true;
		context.Controller.CloseCurrentWindow();
	}
}