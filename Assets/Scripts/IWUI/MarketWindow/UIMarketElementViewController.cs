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
	
	[IWUIBinding] private Image back;
	[IWUIBinding("#ButtonBack")] private Image btnBack;
	
	[IWUIBinding("#LockAnchor")] private Transform lockAnchor;
	
	[IWUIBinding("#Unlock")] private GameObject unlockObj;
	[IWUIBinding("#Lock")] private GameObject lockObj;
	
	private bool isClick;
	private bool isReward;
	private BoardPosition? rewardPosition;

	public override void Init()
	{
		base.Init();
		
		var contentEntity = entity as UIMarketElementEntity;
		
		isClick = false;
		isReward = false;
		rewardPosition = null;

		var isLock = contentEntity.Def.State == MarketItemState.Lock || contentEntity.Def.State == MarketItemState.Claimed;
		
		back.color = new Color(1, 1, 1, isLock ? 0.5f : 1);
		
		unlockObj.SetActive(!isLock);
		lockObj.SetActive(isLock);
		
		if (isLock)
		{
			CreateIcon(lockAnchor, contentEntity.ContentId);
			Sepia = contentEntity.Def.State == MarketItemState.Claimed;
		}

		nameLabel.Text = contentEntity.Def.State != MarketItemState.Lock
			? contentEntity.Name
			: string.Format(LocalizationService.Get("window.market.item.locked.message", "window.market.item.locked.message {0}"), contentEntity.Def.Level);
		
		lockAmountLabel.Text = contentEntity.Def.State == MarketItemState.Lock ? "" : contentEntity.LabelText;
		
		switch (contentEntity.Def.State)
		{
			case MarketItemState.Lock:
				lockMessage.Text = LocalizationService.Get("window.market.item.locked", "window.market.item.locked");
				break;
			case MarketItemState.Claimed:
				lockMessage.Text = LocalizationService.Get("window.market.item.claimed", "window.market.item.claimed");
				break;
			default:
				lockMessage.Text = "";
				break;
		}
		
		ChangeButtons();
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
		base.OnViewCloseCompleted();

		if (!(entity is UIMarketElementEntity contentEntity) || isClick == false || isReward == false || rewardPosition == null) return;
		
		contentEntity.Def.State = MarketItemState.Claimed;

		CurrencyHelper.PurchaseAndProvideSpawn(new List<CurrencyPair> {contentEntity.Def.Reward},
			null,
			rewardPosition,
			() => { BoardService.Current.FirstBoard.TutorialLogic.Update(); },
			false,
			true);
	}
	
	private void ChangeButtons()
	{
		var contentEntity = entity as UIMarketElementEntity;
		
		btnBack.sprite = IconService.Current.GetSpriteById($"button{(contentEntity.Def.State == MarketItemState.Saved ? "Green" : "Blue")}");
		
		btnLabel.Text = contentEntity.Def.State == MarketItemState.Normal
			? string.Format(LocalizationService.Get("common.button.buyFor", "common.button.buyFor {0}"), contentEntity.Def.Current.Price.ToStringIcon())
			: LocalizationService.Get("common.button.claim", "common.button.claim");
	}

	private void OnClickInfo()
	{
		var contentEntity = entity as UIMarketElementEntity;
		var index = GameDataService.Current.MarketManager.Defs.IndexOf(contentEntity.Def);
		
		UIMessageWindowController.CreateDefaultMessage($"Slot {index + 1}");
	}

	private void OnClick()
	{
		if(isClick) return;
		
		isClick = true;
		
		var contentEntity = entity as UIMarketElementEntity;
		
		if (contentEntity.Def.State == MarketItemState.Saved)
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

		if (CurrencyHelper.IsCanPurchase(price, true) == false)
		{
			isClick = false;
			return;
		}
		
		if (price.Currency != Currency.Crystals.Name)
		{
			Paid();
			return;
		}
		
		var model = UIService.Get.GetCachedModel<UIConfirmationWindowModel>(UIWindowType.ConfirmationWindow);
		
		model.IsMarket = true;
		model.Icon = contentEntity.Def.Reward.Currency;
		
		model.Price = price;
		model.Product = contentEntity.Def.Reward;
        
		model.OnAccept = Paid;
		model.OnCancel = () => { isClick = false; };
        
		UIService.Get.ShowWindow(UIWindowType.ConfirmationWindow);
	}
	
	private void Paid()
	{
		var contentEntity = entity as UIMarketElementEntity;
		
		CurrencyHelper.Purchase(Currency.Chest.Name, 1, contentEntity.Def.Current.Price, success =>
		{
			if (success == false)
			{
				isClick = false;
				return;
			}
			
			contentEntity.Def.State = MarketItemState.Purchased;
			NSAudioService.Current.Play(SoundId.BuyMarket);
			AddReward();
		});
	}

	private void AddReward()
	{
		var contentEntity = entity as UIMarketElementEntity;
		var board = BoardService.Current.FirstBoard;

		if (board.BoardLogic.EmptyCellsFinder.CheckFreeSpaceReward(contentEntity.Def.Reward.Amount, true, out var position) == false)
		{
			isClick = false;
			contentEntity.Def.State = MarketItemState.Saved;
			ChangeButtons();
			return;
		}

		rewardPosition = position;
		isReward = true;
		context.Controller.CloseCurrentWindow();
	}
}