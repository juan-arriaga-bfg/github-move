using System;
using System.Collections.Generic;
using System.Linq;
using BfgAnalytics;
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
        IapService.Current.OnInitialized += OnInitialized;
    }
    
    private void Unsubscribe()
    {
        IapService.Current.OnPurchaseOK -= OnPurchaseOk;
        IapService.Current.OnPurchaseFail -= OnPurchaseFail;
        IapService.Current.OnInitialized -= OnInitialized;
    }

    private void OnInitialized()
    {
        
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
        Debug.LogWarning($"[SellForCashManager] => ShowError for {error}");
        
        var model = UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);

        switch (error)
        {
            case IapErrorCode.NoError:
                Debug.LogError($"[SellForCashManager] => ShowError called but error param == NoError");
                return;
            
            case IapErrorCode.InitFailReasonPurchasingUnavailable:
            case IapErrorCode.InitFailReasonNoProductsAvailable:
            case IapErrorCode.InitFailReasonUnknown:
            case IapErrorCode.PurchaseFailReasonPurchasingUnavailable:  
            case IapErrorCode.PurchaseFailIapPrviderNotInitialized:  
                model.Title = LocalizationService.Get("iap.init.failed.title",    "iap.init.failed.title");
                model.Message = LocalizationService.Get("iap.init.failed.message","iap.init.failed.message");
                model.AcceptLabel = LocalizationService.Get("common.button.ok",   "common.button.ok");
                break; 
            
            default:
                model.Title = LocalizationService.Get("iap.failed.title",       "iap.failed.title");
                model.Message = LocalizationService.Get("iap.failed.message",   "iap.failed.message");
                model.AcceptLabel = LocalizationService.Get("common.button.ok", "common.button.ok");
                break;
        }
        
        model.OnAccept = () => { };

        UIService.Get.ShowWindow(UIWindowType.MessageWindow);
    }

    private void OnPurchaseOk(string productId, string receipt, bool restore)
    {
        UIWaitWindowView.Hide();

        if (!restore) // Restore will be handled in RestoredPurchasesProvider
        {
            ProvideReward(productId);
        }
        
        ProfileService.Current.GetComponent<BaseInformationSaveComponent>(BaseInformationSaveComponent.ComponentGuid).IsPayer = true;

        onComplete?.Invoke(true, productId);
        onComplete = null;
    }

    public void ProvideReward(string productId)
    {
        if (!AsyncInitService.Current.IsAllComponentsInited())
        {
            Debug.LogError($"[SellForCashManager] => provideReward: Skip by AsyncInitService.IsAllComponentsInited() == false");
            return;
        }
        
        var defs = GameDataService.Current.ShopManager.Defs[Currency.Crystals.Name];
        var def = defs.FirstOrDefault(e => e.PurchaseKey == productId);

        if (def == null)
        {
            Debug.LogError($"[SellForCashManager] => provideReward: No product with purchase key '{productId}' is defined in GameDataService.Current.ShopManager.Defs[Currency.Crystals.Name]");
            return;
        }
        
        var products = def.Products;
        
        CurrencyHelper.PurchaseAndProvideSpawn(products, null, null, null, false, true);
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

        UIWaitWindowView.Show()
                        .HideOnFocus();
        
        IapService.Current.Purchase(productId);
    }
}