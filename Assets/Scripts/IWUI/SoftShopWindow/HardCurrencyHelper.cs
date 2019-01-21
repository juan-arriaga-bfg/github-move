using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HardCurrencyHelper
{
    private Action<bool, string> onComplete;
    
    public HardCurrencyHelper()
    {
        IapService.Current.OnPurchaseOK += OnPurchaseOk;
        IapService.Current.OnPurchaseFail += OnPurchaseFail;
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

    public static void ProvideReward(string productId)
    {
        List<ShopDef> defs = GameDataService.Current.ShopManager.Defs[Currency.Crystals.Name];
        var def = defs.FirstOrDefault(e => e.PurchaseKey == productId);

        if (def == null)
        {
            Debug.LogError($"ProvideReward: No product with purchase key '{productId}' is defined in GameDataService.Current.ShopManager.Defs[Currency.Crystals.Name]");
            return;
        }
        
        var products = def.Products;
        var price = def.Price;
        
        CurrencyHellper.PurchaseAndProvide(products, price);
        
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