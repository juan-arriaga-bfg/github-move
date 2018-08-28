﻿using DG.Tweening;
using UnityEngine;

public class UILimitPanelViewController : UIGenericResourcePanelViewController
{
    [SerializeField] protected string itemLimitUid;
    
    protected int currentLimitValue;
    
    private StorageItem storageItemLimit;
    public override int CurrentValueAnimated
    {
        get { return currentValueAnimated; }
        set
        {
            currentValueAnimated = value;
            SetLabelText(currentValueAnimated, limitValueAnimated);
        }
    }
    
    protected int limitValueAnimated;
    public virtual int LimitValueAnimated
    {
        get { return limitValueAnimated; }
        set
        {
            limitValueAnimated = value;
            SetLabelText(currentValueAnimated, limitValueAnimated);
        }
    }

    public override void OnViewShow(IWUIWindowView context)
    {
        this.storageItemLimit = ProfileService.Current.Purchases.GetStorageItem(itemLimitUid);
        
        base.OnViewShow(context);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        ShopService.Current.OnPurchasedEvent += OnPurchasedEventHandler;
    }

    protected override void OnDisable()
    {
        ShopService.Current.OnPurchasedEvent -= OnPurchasedEventHandler;
        base.OnDisable();
    }
    
    private void OnPurchasedEventHandler(IPurchaseableItem purchaseableItem, IShopItem shopItem)
    {
        if(shopItem.ItemUid != itemLimitUid) return;
            
        limitValueAnimated = currentLimitValue;
        currentLimitValue += shopItem.Amount;

        DOTween.Kill(storageItemLimit);
        
        var sequence = DOTween.Sequence().SetId(storageItemLimit);
        sequence.Insert(0f, DOTween.To(() => LimitValueAnimated, (v) => { LimitValueAnimated = v; }, currentLimitValue, 0.5f ));
    }
    
    public override void UpdateView()
    {
        if (storageItem == null) return;

        currentValueAnimated = currentValue = storageItem.Amount;
        limitValueAnimated = currentLimitValue = storageItemLimit.Amount;

        SetLabelText(storageItem.Amount, storageItemLimit.Amount);
        
        if (icon != null) icon.sprite = IconService.Instance.Manager.GetSpriteById(string.Format(IconPattern, storageItem.Currency));
    }
    
    private void SetLabelText(int current, int limit)
    {
        if (amountLabel == null) return;
        amountLabel.Text = $"<mspace=2.7em>{current}/{limit}</mspace>";
    }
    
    public void DebugCurrentResources()
    {
        BoardService.Current.GetBoardById(0)?.BoardEvents.RaiseEvent(GameEventsCodes.ClosePieceUI, this);
        UIService.Get.ShowWindow(UIWindowType.EnergyShopWindow);
    }
}