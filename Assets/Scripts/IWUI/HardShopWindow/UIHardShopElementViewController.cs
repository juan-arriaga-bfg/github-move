using UnityEngine;
using UnityEngine.UI;

public class UIHardShopElementViewController : UIShopElementViewController
{
    [IWUIBinding] protected Image back;
    [IWUIBinding("#ClaimName")] protected NSText claimName;
    
    protected bool isClaimed;

    public override void Init()
    {
        base.Init();
        
        var contentEntity = entity as UIShopElementEntity;
        
        claimName.Text = contentEntity.NameLabel;
    }

    public override void OnViewCloseCompleted()
    {
        isClaimed = false;
        base.OnViewCloseCompleted();
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

        back.color = new Color(1, 1, 1, isClaimed ? 0.5f : 1);
    }

    protected override void OnPurchaseComplete()
    {
        base.OnPurchaseComplete();
        
        var contentEntity = entity as UIShopElementEntity;
        
        if (contentEntity.IsPermanent == false)
        {
            isClaimed = true;
            ChangeView();
        }
    }
}