using System.Collections.Generic;
using BfgAnalytics;
using UnityEngine;
using UnityEngine.UI;

public class UIEnergyShopElementViewController : UISimpleScrollElementViewController
{
    [IWUIBinding("#NameLabel")] private NSText nameLabel;
    [IWUIBinding("#BuyButtonLabel")] private NSText btnBuyLabel;
    [IWUIBinding("#LockButtonLabel")] private NSText btnLockLabel;
    
    [IWUIBinding("#LockLabel")] private NSText lockLabel;
    [IWUIBinding("#LockMessage")] private NSText lockMessage;
    [IWUIBinding("#LockName")] private NSText lockName;
    
    [IWUIBinding("#FreeLabel")] private NSText freeLabel;
	
    [IWUIBinding("#BuyButton")] private UIButtonViewController btnBuy;
	
    [IWUIBinding] private Image back;
    [IWUIBinding("#ButtonBack")] private Image btnBack;
	
    [IWUIBinding("#LockAnchor")] private Transform lockAnchor;
	
    [IWUIBinding("#Unlock")] private GameObject unlockObj;
    [IWUIBinding("#Lock")] private GameObject lockObj;
    [IWUIBinding("#Free")] private GameObject freeObj;
    [IWUIBinding("#Free")] private CanvasGroup freeCanvas;
    
    private bool isClick;
    private bool isClaimed;
    
    public override void Init()
    {
	    base.Init();
	    
	    var contentEntity = entity as UIShopElementEntity;
		
	    isClick = false;
	    isClaimed = false;
	    
	    if (contentEntity.Price != null) ChangeButtons();

	    nameLabel.Text = lockName.Text = contentEntity.NameLabel;
	    lockLabel.Text = label.Text;
	    lockMessage.Text = LocalizationService.Get("common.message.comingSoon", "common.message.comingSoon");
	    freeLabel.Text = LocalizationService.Get("common.button.free", "common.button.free");
	    
	    ChangeView();
	    freeObj.SetActive(contentEntity.Price != null && contentEntity.Price.Amount == 0);
    }
    
    public override void OnViewShowCompleted()
    {
	    base.OnViewShowCompleted();
		
	    btnBuy
		    .ToState(GenericButtonState.Active)
		    .OnClick(OnClick);
    }

    private void ChangeView()
    {
	    var contentEntity = entity as UIShopElementEntity;
	    var isLock = contentEntity.Price == null;
	    var key = $"window.shop.energy.item.{(isClaimed ? "claimed" : "locked")}";
	    
	    unlockObj.SetActive(!isLock && !isClaimed);
	    lockObj.SetActive(isLock || isClaimed);
	    
	    lockLabel.gameObject.SetActive(isClaimed);
	    lockMessage.gameObject.SetActive(!isClaimed);
	    lockName.gameObject.SetActive(isClaimed);
	    btnLockLabel.Text = LocalizationService.Get(key, key);

	    if (isLock || isClaimed) CreateIcon(lockAnchor, isLock ? PieceType.Empty.Abbreviations[0] : contentEntity.ContentId);
	    if (isClaimed) Sepia = true;

	    freeCanvas.alpha = isClaimed ? 0.5f : 1;
	    back.color = new Color(1, 1, 1, isClaimed ? 0.5f : 1);
    }
    
    private void ChangeButtons()
    {
	    var contentEntity = entity as UIShopElementEntity;
	    var isFree = contentEntity.Price.Amount == 0;
		
	    btnBack.sprite = IconService.Current.GetSpriteById($"button{(isFree ? "Green" : "Blue")}");
		
	    btnBuyLabel.Text = isFree
		    ? LocalizationService.Get("common.button.claim", "common.button.claim")
		    : string.Format(LocalizationService.Get("common.button.buy", "common.button.buy {0}"), contentEntity.Price.ToStringIcon());
    }
    
    private void OnClick()
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

	    if (contentEntity.IsPermanent == false)
	    {
		    isClaimed = true;
		    ChangeView();
	    }
	    
	    Analytics.SendPurchase("screen_energy", $"item{CachedTransform.GetSiblingIndex()}", new List<CurrencyPair>{contentEntity.Price}, new List<CurrencyPair>(contentEntity.Products), false, contentEntity.Price.Amount == 0);
    }
}