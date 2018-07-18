using UnityEngine;

public class UILimitPanelViewController : UIGenericResourcePanelViewController
{
    [SerializeField] protected string itemLimitUid;
    
    private StorageItem storageItemLimit;
    
    public override int CurrentValueAnimated
    {
        get { return currentValueAnimated; }
        set
        {
            currentValueAnimated = value;

            if (amountLabel != null) amountLabel.Text = UpdateLabel(currentValueAnimated);
        }
    }

    public override void OnViewShow(IWUIWindowView context)
    {
        this.storageItemLimit = ProfileService.Current.Purchases.GetStorageItem(itemLimitUid);
        
        base.OnViewShow(context);
    }

    public override void UpdateView()
    {
        if (storageItem == null) return;

        currentValue = storageItem.Amount;

        if (amountLabel != null) amountLabel.Text = UpdateLabel(storageItem.Amount);
        if (icon != null) icon.sprite = IconService.Instance.Manager.GetSpriteById(string.Format(IconPattern, storageItem.Currency));
    }

    private string UpdateLabel(int value)
    {
        return string.Format("{0}/{1}", value, storageItemLimit.Amount);
    }
    
    public void DebugCurrentResources()
    {
        UIService.Get.ShowWindow(UIWindowType.EnergyShopWindow);
    }
}