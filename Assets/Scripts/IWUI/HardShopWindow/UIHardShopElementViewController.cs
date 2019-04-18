using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHardShopElementViewController : UIShopElementViewController
{
    [IWUIBinding] protected Image back;
    
    [IWUIBindingNullable("#Offer")] protected GameObject offer;
    [IWUIBindingNullable("#Offer")] protected Image offerBack;
    
    [IWUIBinding("#ClaimName")] protected NSText claimName;
    
    [IWUIBindingNullable("#Product1")] protected NSText product1;
    [IWUIBindingNullable("#Product2")] protected NSText product2;
    [IWUIBindingNullable("#Product3")] protected NSText product3;
    [IWUIBindingNullable("#Product4")] protected NSText product4;

    private List<NSText> productLabels;
    
    private VertexGradient hardGradient;
    private VertexGradient otherGradient;
    
    protected bool isClaimed;
    private bool isOffer;

    public override void Init()
    {
        base.Init();
        
        ColorUtility.TryParseHtmlString("#FF5FEF", out var hardTop);
        ColorUtility.TryParseHtmlString("#E17AFF", out var hardBottom);
        
        ColorUtility.TryParseHtmlString("#FFD45F", out var otherTop);
        ColorUtility.TryParseHtmlString("#FF9241", out var otherBottom);
        
        hardGradient = new VertexGradient(hardTop, hardTop, hardBottom, hardBottom);
        otherGradient = new VertexGradient(otherTop, otherTop, otherBottom, otherBottom);
        
        var contentEntity = entity as UIShopElementEntity;

        isOffer = contentEntity.Products.Count > 1;
        claimName.Text = isOffer ? LocalizationService.Get("window.shop.offer.thank", "window.shop.offer.thank") : contentEntity.NameLabel;
        
        if (isOffer == false) return;
        
        productLabels = new List<NSText>
        {
            product1,
            product2,
            product3,
            product4
        };

        for (var i = 0; i < productLabels.Count; i++)
        {
            var item = productLabels[i];
            var isActive = i < contentEntity.Products.Count;
            
            item.gameObject.SetActive(isActive);

            if (isActive == false) continue;

            var product = contentEntity.Products[i];
            var isCrystals = product.Currency == Currency.Crystals.Name;

            item.Text = product.ToStringIcon();
            item.TextLabel.colorGradient = isCrystals ? hardGradient : otherGradient;
            item.StyleId = isCrystals ? 23 : 24;
            item.ApplyStyle();
        }
        
        BoardService.Current.FirstBoard.MarketLogic.OfferTimer.OnTimeChanged += UpdateTimer;
        UpdateTimer();
    }

    public override void OnViewCloseCompleted()
    {
        isClaimed = false;
        
        base.OnViewCloseCompleted();

        if (isOffer == false) return;
        
        BoardService.Current.FirstBoard.MarketLogic.OfferTimer.OnTimeChanged -= UpdateTimer;
    }
    
    protected override void ChangeView()
    {
        var contentEntity = entity as UIShopElementEntity;
        var key = $"window.shop.energy.item.{(isClaimed ? "claimed" : "locked")}";
	    
        unlockObj.SetActive(!isLock && !isClaimed);
        lockObj.SetActive(isLock || isClaimed);
	    
        lockMessage.gameObject.SetActive(!isClaimed);
        claimName.gameObject.SetActive(isClaimed);
	    
        btnLockLabel.Text = LocalizationService.Get(key, key);
	    
        CreateIcon(isLock || isClaimed ? lockAnchor : anchor, isLock ? PieceType.Empty.Abbreviations[0] : contentEntity.ContentId);

        if (isClaimed) Sepia = true;
        if (offer != null) {offer.SetActive(contentEntity.Products.Count > 1);}

        back.color = new Color(1, 1, 1, isClaimed ? 0.5f : 1);
        if(offerBack != null) offerBack.color = new Color(1, 1, 1, isClaimed ? 0.75f : 1);
        
        if (productLabels != null)
        {
            foreach (var text in productLabels)
            {
                text.TextLabel.alpha = isClaimed ? 0.5f : 1;
            }
        }
    }

    protected override void OnPurchaseComplete(bool isIap)
    {
        if (isOffer)
        {
            BoardService.Current.FirstBoard.MarketLogic.CompleteOffer();
            BoardService.Current.FirstBoard.TutorialLogic.UpdateHard();
            BoardService.Current.FirstBoard.MarketLogic.OfferTimer.OnTimeChanged -= UpdateTimer;
            label.Text = LocalizationService.Get("common.message.sold", "common.message.sold");
        }
        
        base.OnPurchaseComplete(isIap);
        
        var contentEntity = entity as UIShopElementEntity;

        if (contentEntity.IsPermanent) return;
        
        isClaimed = true;
        ChangeView();
    }
    
    private void UpdateTimer()
    {
        nameLabel.Text = BoardService.Current.FirstBoard.MarketLogic.OfferTimer.CompleteTime.GetTimeLeftText(true, false, null, false,  true);
    }
}