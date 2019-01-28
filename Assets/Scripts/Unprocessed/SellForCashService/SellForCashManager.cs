using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SellForCashManager: ECSEntity
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();

    public override int Guid => ComponentGuid;
    
    private Action<bool, string> onComplete;

    private bool inited;

    public void Init()
    {
        if (inited)
        {
            return;
        }

        inited = true;
        Subscribe();
    }
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        base.OnRegisterEntity(entity);
        Init();
    }

    private void Subscribe()
    {
        IapService.Current.OnPurchaseOK += OnPurchaseOk;
        IapService.Current.OnPurchaseFail += OnPurchaseFail;
    }
    
    private void Unsubscribe()
    {
        IapService.Current.OnPurchaseOK -= OnPurchaseOk;
        IapService.Current.OnPurchaseFail -= OnPurchaseFail;
    }

    public override void OnUnRegisterEntity(ECSEntity entity)
    {
        base.OnUnRegisterEntity(entity);
        Cleanup();
    }

    public void Cleanup()
    {
        Unsubscribe();
        inited = false;
    }

    private void OnPurchaseFail(string productId, IapErrorCode error)
    {
        UIWaitWindowView.Hide();
        onComplete?.Invoke(false, productId);
        onComplete = null;

        ShowError(error);
    }

    private void ShowError(IapErrorCode error)
    {
        var model = UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);

        model.Title = LocalizationService.Get("iap.failed.title",       "iap.failed.title");
        model.Message = LocalizationService.Get("iap.failed.message",   "iap.failed.message");
        model.AcceptLabel = LocalizationService.Get("common.button.ok", "common.button.ok");

        model.OnAccept = () => { };

        UIService.Get.ShowWindow(UIWindowType.MessageWindow);
    }

    private void OnPurchaseOk(string productId, string receipt)
    {
        UIWaitWindowView.Hide();
        
        ProvideReward(productId);
        
        onComplete?.Invoke(true, productId);
        onComplete = null;
    }

    public void ProvideReward(string productId)
    {
        var defs = GameDataService.Current.ShopManager.Defs[Currency.Crystals.Name];
        var def = defs.FirstOrDefault(e => e.PurchaseKey == productId);

        if (def == null)
        {
            Debug.LogError($"ProvideReward: No product with purchase key '{productId}' is defined in GameDataService.Current.ShopManager.Defs[Currency.Crystals.Name]");
            return;
        }
        
        var products = def.Products;
        var price = def.Price;
        
        CurrencyHelper.PurchaseAndProvideSpawn(products, price, null, null, false, true);
        IapService.Current.IapProvidedToPlayer(productId);
    }

    public void Purchase(string productId, Action<bool, string> onComplete)
    {
        this.onComplete = onComplete;
        
        if (!NetworkUtils.CheckInternetConnection(true))
        {
            this.onComplete?.Invoke(false, productId);
            this.onComplete = null;
            return;
        }
        
        UIWaitWindowView.Show();
        IapService.Current.Purchase(productId);
    }
}