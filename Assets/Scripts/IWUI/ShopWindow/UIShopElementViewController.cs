using System.Collections.Generic;
using System.Linq;
using BfgAnalytics;
using UnityEngine;
using UnityEngine.UI;

public class UIShopElementViewController : UISimpleScrollElementViewController
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
		    : contentEntity.ButtonLabel;
    }
    
    private void OnClick()
    {
	    if (isClick) return;
		
	    isClick = true;
	    
	    var contentEntity = entity as UIShopElementEntity;
	    
	    if (contentEntity.Price.Currency == Currency.Cash.Name)
	    {
		    OnBuyUsingCash();
		    return;
	    }

	    if (CurrencyHelper.IsCanPurchase(contentEntity.Price, true) == false)
	    {
		    isClick = false;
		    return;
	    }

	    if (contentEntity.Price.Currency == Currency.Crystals.Name)
	    {
		    Confirmation();
		    return;
	    }

	    OnPurchase(null);
    }

    private void OnBuyUsingCash()
    {
	    var contentEntity = entity as UIShopElementEntity;
        
	    // HACK to handle the case when we have a purchase but BFG still not add it to the Store
	    var isDefRegister = IapService.Current.IapCollection.Defs.All(e => e.Id != contentEntity.PurchaseKey);
	    if (isDefRegister || DevTools.IsIapEnabled() == false)
	    {
		    var model = UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);

		    model.Title = "[DEBUG]";
		    model.Message = $"Product with id '{contentEntity.PurchaseKey}' not registered. Purchase will be processed using debug flow without real store.";
		    model.AcceptLabel = LocalizationService.Get("common.button.ok", "common.button.ok");
		    model.OnAccept = () => OnPurchase(null);
		    model.OnClose = () => { isClick = false; };

		    UIService.Get.ShowWindow(UIWindowType.MessageWindow);
		    return;
	    }
	    // END
	    
	    CurrencyHelper.FlyPosition = GetComponentInParent<Canvas>().worldCamera.WorldToScreenPoint(btnBack.transform.position);
	    
	    SellForCashService.Current.Purchase(contentEntity.PurchaseKey, (isOk, productId) =>
	    {
		    isClick = false;

		    if (isOk)
		    {
			    OnPurchaseComplete(true);
		    }
	    });
    }

    private void Confirmation()
    {
        if (UIService.Get.GetShowedWindowByName(UIWindowType.ConfirmationWindow) != null)
        {
            isClick = false;
            return;
        }
        
	    var contentEntity = entity as UIShopElementEntity;
	    var model = UIService.Get.GetCachedModel<UIConfirmationWindowModel>(UIWindowType.ConfirmationWindow);
	    
	    model.Icon = contentEntity.ContentId;
	    
	    model.ButtonText = string.Format(LocalizationService.Get("common.button.buyFor", "common.button.buyFor {0}"), contentEntity.Price.ToStringIcon());
	    model.ProductAmountText = contentEntity.LabelText;
	    model.ProductNameText = contentEntity.NameLabel;
	    
	    model.OnAccept = OnPurchase;
	    model.OnCancel = () => { isClick = false; };

	    UIService.Get.ShowWindow(UIWindowType.ConfirmationWindow);
    }

    private void OnPurchase(Transform anchor)
    {
	    var position = anchor == null ? btnBack.transform.position : anchor.position;
	    var contentEntity = entity as UIShopElementEntity;
	    var flyPosition = GetComponentInParent<Canvas>().worldCamera.WorldToScreenPoint(position);
	    
	    var products = new List<CurrencyPair>(contentEntity.Products);
	    if (contentEntity.Extras != null) products.AddRange(contentEntity.Extras);
	    
	    CurrencyHelper.PurchaseAsyncOnlyCurrency(products, contentEntity.Price, flyPosition, null);
	    
	    isClick = false;
	    OnPurchaseComplete(false);
    }

    protected virtual void OnPurchaseComplete(bool isIap)
    {
	    ProfileService.Instance.Manager.UploadCurrentProfile(isIap);
	    SendAnalyticsEvent();
    }
    
    protected void SendAnalyticsEvent()
    {
	    var shopModel = context.Model as UIShopWindowModel;
	    var contentEntity = entity as UIShopElementEntity;
	    
	    var products = new List<CurrencyPair>(contentEntity.Products);
	    
	    var isFree = contentEntity.Price != null && contentEntity.Price.Currency != Currency.Cash.Name && contentEntity.Price.Amount == 0;
	    var isIap = contentEntity.Price != null && contentEntity.Price.Currency == Currency.Cash.Name;
	    var isOffer = products.Count > 1;
	    
	    if (contentEntity.Extras != null) products.AddRange(contentEntity.Extras);
	    
	    Analytics.SendPurchase(
		    isOffer ? $"{shopModel.AnalyticLocation}_{Currency.Offer.Name.ToLower()}" : shopModel.AnalyticLocation,
		    shopModel.AnalyticReason(contentEntity.Def),
		    new List<CurrencyPair>{contentEntity.Price},
		    products, isIap, isFree);
    }
}