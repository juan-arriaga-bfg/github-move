using System.Collections.Generic;
using BfgAnalytics;
using UnityEngine;
using UnityEngine.UI;

public class UISoftShopElementViewController : UISimpleScrollElementViewController
{
    [IWUIBinding("#NameLabel")] protected NSText nameLabel;
    [IWUIBinding("#BuyButtonLabel")] protected NSText btnBuyLabel;
    [IWUIBinding("#LockButtonLabel")] protected NSText btnLockLabel;
    
    [IWUIBinding("#LockMessage")] protected NSText lockMessage;
    
    [IWUIBinding("#BuyButton")] protected UIButtonViewController btnBuy;
	
    [IWUIBinding("#ButtonBack")] protected Image btnBack;
	
    [IWUIBinding("#LockAnchor")] protected Transform lockAnchor;
	
    [IWUIBinding("#Unlock")] protected GameObject unlockObj;
    [IWUIBinding("#Lock")] protected GameObject lockObj;
    
    protected bool isClick;
    protected bool isLock;
    
    public override void Init()
    {
	    base.Init();
	    
	    var contentEntity = entity as UIShopElementEntity;
		
	    isClick = false;
	    isLock = contentEntity.Price == null;
	    
	    if (isLock == false) ChangeButtons(contentEntity.Price.Amount == 0);

	    nameLabel.Text = contentEntity.NameLabel;
	    lockMessage.Text = LocalizationService.Get("common.message.comingSoon", "common.message.comingSoon");
	    btnLockLabel.Text = LocalizationService.Get("window.shop.energy.item.locked", "window.shop.energy.item.locked");
	    
	    ChangeView();
    }
    
    public override void OnViewShowCompleted()
    {
	    base.OnViewShowCompleted();
		
	    btnBuy
		    .ToState(GenericButtonState.Active)
		    .OnClick(OnClick);
    }

    protected virtual void ChangeView()
    {
	    var contentEntity = entity as UIShopElementEntity;
	    
	    unlockObj.SetActive(!isLock);
	    lockObj.SetActive(isLock);
	    
	    CreateIcon(isLock ? lockAnchor : anchor, isLock ? PieceType.Empty.Abbreviations[0] : contentEntity.ContentId);
    }
    
    private void ChangeButtons(bool isFree)
    {
	    var contentEntity = entity as UIShopElementEntity;

	    btnBack.sprite = IconService.Current.GetSpriteById($"button{(isFree ? "Green" : "Blue")}");
		
	    btnBuyLabel.Text = isFree
		    ? LocalizationService.Get("common.button.claim", "common.button.claim")
		    : string.Format(LocalizationService.Get("common.button.buy", "common.button.buy {0}"), contentEntity.Price.ToStringIcon());
    }
    
    protected void OnClick()
    {
	    if (isClick) return;
		
	    isClick = true;
	    
	    var contentEntity = entity as UIShopElementEntity;

	    if (CurrencyHelper.IsCanPurchase(contentEntity.Price, true) == false)
	    {
		    isClick = false;
		    return;
	    }
	    
	    var flyPosition = GetComponentInParent<Canvas>().worldCamera.WorldToScreenPoint(btnBack.transform.position);
	    var transaction = CurrencyHelper.PurchaseAsync(contentEntity.Products[0], contentEntity.Price, success =>
	    {
		    if (success == false ) return;
		    
		    isClick = false;
	    }, flyPosition);
	    
	    transaction.Complete();
	    OnPurchaseComplete();
    }

    protected virtual void OnPurchaseComplete()
    {
	    SendAnalyticsEvent();
    }

    protected void SendAnalyticsEvent(bool isIap = false, bool isFree = false)
    {
	    var shopModel = context.Model as UIShopWindowModel;
	    var contentEntity = entity as UIShopElementEntity;
	    
	    Analytics.SendPurchase(shopModel.AnalyticLocation, $"item{CachedTransform.GetSiblingIndex()}", new List<CurrencyPair>{contentEntity.Price}, new List<CurrencyPair>(contentEntity.Products), isIap, isFree);
    }
}