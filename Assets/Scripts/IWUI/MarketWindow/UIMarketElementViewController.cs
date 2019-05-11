using System;
using System.Collections.Generic;
using System.Linq;
using BfgAnalytics;
using UnityEngine;
using UnityEngine.UI;

public class UIMarketElementViewController : UISimpleScrollElementViewController
{
	[IWUIBinding("#NameLabel")] private NSText nameLabel;
	[IWUIBinding("#ButtonLabel")] private NSText btnLabel;
	[IWUIBinding("#LockLabel")] private NSText lockAmountLabel;
	[IWUIBinding("#LockMessage")] private NSText lockMessage;
	[IWUIBinding("#LockLevelMessage")] private NSText lockLevelMessage;
	
	[IWUIBinding("#BuyButton")] private UIButtonViewController btnBuy;
	[IWUIBinding("#ButtonInfo")] private UIButtonViewController btnInfo;
	
	[IWUIBinding] private Image back;
	[IWUIBinding("#ButtonBack")] private Image btnBack;
	
	[IWUIBinding("#LockAnchor")] private Transform lockAnchor;
	
	[IWUIBinding("#Unlock")] private GameObject unlockObj;
	[IWUIBinding("#Lock")] private GameObject lockObj;
	[IWUIBinding("#Permanent")] private GameObject permanentBack;
	
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
		permanentBack.SetActive(isLock == false && contentEntity.Def.Current.IsPermanent);
		
		if (isLock)
		{
			CreateIcon(lockAnchor, contentEntity.ContentId);
			Sepia = contentEntity.Def.State == MarketItemState.Claimed;
		}

		lockAmountLabel.Text = contentEntity.Def.State == MarketItemState.Lock ? "" : contentEntity.LabelText;
        
        nameLabel.Text = contentEntity.Name;
        
		switch (contentEntity.Def.State)
		{
			case MarketItemState.Lock:
				lockMessage.Text = LocalizationService.Get("window.market.item.locked", "window.market.item.locked");
                lockLevelMessage.gameObject.SetActive(true);
                lockLevelMessage.Text = string.Format(LocalizationService.Get("window.market.item.locked.message", "window.market.item.locked.message {0}"), contentEntity.Def.Level);
                nameLabel.Text = "";
				break;
            
			case MarketItemState.Claimed:
				lockMessage.Text = LocalizationService.Get("window.market.item.claimed", "window.market.item.claimed");
                lockLevelMessage.gameObject.SetActive(false);
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
		
		void Complete()
		{
			ProfileService.Instance.Manager.UploadCurrentProfile(false);
			BoardService.Current.FirstBoard.TutorialLogic.Update();
		}
		
		var flyPosition = (context as UIBaseWindowView).GetCanvas().worldCamera.WorldToScreenPoint(btnBuy.transform.position);
		
		CurrencyHelper.PurchaseAndProvideSpawn(new List<CurrencyPair>{contentEntity.Def.Reward},null, rewardPosition, flyPosition, Complete, false, true);
	}
	
	private void ChangeButtons()
	{
		var contentEntity = entity as UIMarketElementEntity;
		
		btnBack.sprite = IconService.Current.GetSpriteById($"button{(contentEntity.Def.State == MarketItemState.Saved ? "Green" : "Blue")}");
		
		btnLabel.Text = contentEntity.Def.State == MarketItemState.Normal
			? string.Format(LocalizationService.Get("common.button.buyFor", "common.button.buyFor {0}"), contentEntity.Def.Price.ToStringIcon())
			: LocalizationService.Get("common.button.claim", "common.button.claim");
	}

	private void OnClickInfo()
	{
		
#if DEBUG
		UIMessageWindowController.CreateDefaultMessage($"Slot {GetIndex()}");
		return;
#endif
		
		var contentEntity = entity as UIMarketElementEntity;

		if (contentEntity.Def.IsPiece == false)
		{
			UIMessageWindowController.CreateMessage(
				LocalizationService.Get("window.market.description.title", "window.market.description.title"),
				contentEntity.Def.Description);
			
			return;
		}

		var def = PieceType.GetDefById(PieceType.Parse(contentEntity.Def.Reward.Currency));

		if (def.Filter.Has(PieceTypeFilter.Chest) == false)
		{
			UIMessageWindowController.CreateMessage(
				LocalizationService.Get("window.market.description.title", "window.market.description.title"),
				contentEntity.Def.Description);
			
			return;
		}
		
		UILootBoxWindowController.OpenChestWindow(def.Id);
	}

	private void OnClick()
	{
		if (isClick) return;
		
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
		var price = contentEntity.Def.Price;

		if (CurrencyHelper.IsCanPurchase(price, true) == false)
		{
			isClick = false;

			if (price.Currency != Currency.Coins.Name) return;
			
			foreach (var tab in panel.Tabs)
			{
				if((tab.Entity as UIMarketElementEntity).Def.Reward.Currency != Currency.Coins.Name) continue;
					
				(context as UIMarketWindowView).Scroll(tab.Index);
				return;
			}
		}
		
		if (price.Currency != Currency.Crystals.Name)
		{
			Paid(null);
			return;
		}
		
		var model = UIService.Get.GetCachedModel<UIConfirmationWindowModel>(UIWindowType.ConfirmationWindow);

		model.Icon = contentEntity.Def.Icon;
		
		model.ButtonText = string.Format(LocalizationService.Get("common.button.buyFor", "common.button.buyFor {0}"), price.ToStringIcon());
		model.ProductAmountText = $"x{contentEntity.Def.Reward.Amount}";
		model.ProductNameText = LocalizationService.Get(contentEntity.Def.Name, contentEntity.Def.Name);
		
		model.OnAccept = Paid;
		model.OnCancel = () => { isClick = false; };
        
		UIService.Get.ShowWindow(UIWindowType.ConfirmationWindow);
	}
	
	private void Paid(Transform anchorPaid)
	{
		var contentEntity = entity as UIMarketElementEntity;
		
		if (contentEntity.Def.IsPiece == false)
		{
			var position = anchorPaid == null ? btnBack.transform.position : anchorPaid.position;
			var flyPosition = GetComponentInParent<Canvas>().worldCamera.WorldToScreenPoint(position);
			
			CurrencyHelper.PurchaseAsyncOnlyCurrency(contentEntity.Def.Reward, contentEntity.Def.Price, flyPosition,
				(success) =>
				{
					if (success)
					{
						ProfileService.Instance.Manager.UploadCurrentProfile(false);
						
                        BoardService.Current.FirstBoard.BoardEvents.RaiseEvent(GameEventsCodes.PurchaseInMarket, contentEntity.Def);
					}
				});
			
			contentEntity.Def.State = MarketItemState.Claimed;
			Analytics.SendPurchase($"market{GetIndex()}", $"item{contentEntity.Def.Index + 1}", new List<CurrencyPair>{contentEntity.Def.Price}, new List<CurrencyPair>{contentEntity.Def.Reward}, false, false);
			Init();
			
			return;
		}
		
		Analytics.SendPurchase($"market{GetIndex()}", $"item{contentEntity.Def.Index + 1}", new List<CurrencyPair>{contentEntity.Def.Price}, null, false, false);
		CurrencyHelper.Purchase(Currency.Market.Name, 1, contentEntity.Def.Price, success =>
		{
			if (success == false)
			{
				isClick = false;
				return;
			}
			
			contentEntity.Def.State = MarketItemState.Purchased;
			NSAudioService.Current.Play(SoundId.BuyMarket);
			AddReward();
			
            BoardService.Current.FirstBoard.BoardEvents.RaiseEvent(GameEventsCodes.PurchaseInMarket, contentEntity.Def);
		});
	}

	private void AddReward()
	{
		var contentEntity = entity as UIMarketElementEntity;
		var board = BoardService.Current.FirstBoard;
		
		context.Controller.CloseCurrentWindow();
		
		var piecesReward = CurrencyHelper.FiltrationRewards(new List<CurrencyPair> {contentEntity.Def.Reward}, out _);
		
		if (board.BoardLogic.EmptyCellsFinder.CheckFreeSpaceReward(piecesReward.Sum(pair => pair.Value), out var position) == false)
		{
			contentEntity.Def.State = MarketItemState.Claimed;
			BoardService.Current.FirstBoard.BoardLogic.AirShipLogic.Add(piecesReward);
			return;
		}

		rewardPosition = position;
		isReward = true;
	}
	
	protected virtual int GetIndex()
	{
		var contentEntity = entity as UIMarketElementEntity;
		return contentEntity.Def.Uid;
	}
}