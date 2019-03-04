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

	    OnPurchase();
    }

    private void OnBuyUsingCash()
    {
	    var contentEntity = entity as UIShopElementEntity;
        
	    // HACK to handle the case when we have a purchase but BFG still not add it to the Store
	    if (IapService.Current.IapCollection.Defs.All(e => e.Id != contentEntity.PurchaseKey))
	    {
		    var model = UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);

		    model.Title = "[DEBUG]";
		    model.Message = $"Product with id '{contentEntity.PurchaseKey}' not registered. Purchase will be processed using debug flow without real store.";
		    model.AcceptLabel = LocalizationService.Get("common.button.ok", "common.button.ok");
		    model.OnAccept = OnPurchase;
		    model.OnClose = () => { isClick = false; };

		    UIService.Get.ShowWindow(UIWindowType.MessageWindow);
		    return;
	    }
	    // END

	    SellForCashService.Current.Purchase(contentEntity.PurchaseKey, (isOk, productId) =>
	    {
		    isClick = false;
		    
		    if (isOk) SendAnalyticsEvent();
	    });
    }

    private void Confirmation()
    {
	    var contentEntity = entity as UIShopElementEntity;
	    var model = UIService.Get.GetCachedModel<UIConfirmationWindowModel>(UIWindowType.ConfirmationWindow);
	    
	    model.Icon = contentEntity.ContentId;
	    
	    model.ButtonText = string.Format(LocalizationService.Get("common.button.buyFor", "common.button.buyFor {0}"), contentEntity.Price.ToStringIcon());
	    model.ProductAmountText = contentEntity.LabelText;
	    model.ProductNameText = contentEntity.NameLabel;
	    
	    model.OnAcceptTap = OnPurchase;
	    model.OnCancel = () => { isClick = false; };

	    UIService.Get.ShowWindow(UIWindowType.ConfirmationWindow);
    }

    protected void OnPurchase()
    {
	    var contentEntity = entity as UIShopElementEntity;
	    var flyPosition = GetComponentInParent<Canvas>().worldCamera.WorldToScreenPoint(btnBack.transform.position);
	    var transaction = CurrencyHelper.PurchaseAsync(contentEntity.Products[0], contentEntity.Price, success =>
	    {
		    if (success == false ) return;
		    
		    isClick = false;
	    }, flyPosition);
	    
	    transaction.Complete();
	    OnPurchaseComplete();
	    PlaySoundOnPurchase(contentEntity.Products);
    }

    protected virtual void OnPurchaseComplete()
    {
	    SendAnalyticsEvent();
    }
    
    private void PlaySoundOnPurchase(List<CurrencyPair> products)
    {
	    foreach (var product in products)
	    {
		    if(product.Currency == Currency.Energy.Name)
			    NSAudioService.Current.Play(SoundId.BuyEnergy, false, 1);
		    if(product.Currency == Currency.Coins.Name)
			    NSAudioService.Current.Play(SoundId.BuySoftCurr, false, 1);    
	    }
    }

    protected void SendAnalyticsEvent()
    {
	    var shopModel = context.Model as UIShopWindowModel;
	    var contentEntity = entity as UIShopElementEntity;

	    var isFree = contentEntity.Price != null && contentEntity.Price.Currency != Currency.Cash.Name && contentEntity.Price.Amount == 0;
	    var isIap = contentEntity.Price != null && contentEntity.Price.Currency == Currency.Cash.Name;
		    
	    
	    Analytics.SendPurchase(shopModel.AnalyticLocation, $"item{CachedTransform.GetSiblingIndex()}", new List<CurrencyPair>{contentEntity.Price}, new List<CurrencyPair>(contentEntity.Products), isIap, isFree);
    }
}