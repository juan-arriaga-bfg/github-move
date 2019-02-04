#if BFG_PURCHASES

using System;
using DG.Tweening;
using UnityEngine;

public class BfgIapProvider : IapProvider
{
    private Action<IapErrorCode> onInitComplete;
    
    private int remainingToRestore;
    
    public override void Init(Action<IapErrorCode> onComplete)
    {
        onInitComplete = onComplete;
        
        PurchaseControllerProductIds.ClearAllLists();
        
        foreach (var item in IapCollection.Defs)
        {
            if (item.Consumable)
            {
                PurchaseControllerProductIds.consumableGoogleProductIds.Add(item.GooglePlayId);
                PurchaseControllerProductIds.consumableIOSProductIds.Add(item.AppleAppStoreId);
                PurchaseControllerProductIds.consumableAmazonProductIds.Add(item.AmazonAppStoreId);
            }
            else
            {
                PurchaseControllerProductIds.nonConsumableGoogleProductIds.Add(item.GooglePlayId);
                PurchaseControllerProductIds.nonConsumableIOSProductIds.Add(item.AppleAppStoreId);
                PurchaseControllerProductIds.nonConsumableAmazonProductIds.Add(item.AmazonAppStoreId);
            }
        }
        
#if !UNITY_EDITOR
        PurchaseController.StartService();
            
        PurchaseController.Instance.PurchaseSucceededEvent += OnPurchaseSuccessCallback;
        PurchaseController.Instance.PurchaseFailedEvent    += OnPurchaseFailedCallback;
        PurchaseController.Instance.RestoreSucceededEvent  += OnRestoreSucceededCallback;
        PurchaseController.Instance.RestoreFailedEvent     += OnRestoreFailedCallback;

    #if UNITY_ANDROID
        if (!NotificationCenter.Instance.HandlerSetHasObserver (purchase_succeeded_with_receipt, bfgPurchaseAndroid.NOTIFICATION_PURCHASE_SUCCEEDED_WITH_RECEIPT)) {
            NotificationCenter.Instance.AddObserver (purchase_succeeded_with_receipt, bfgPurchaseAndroid.NOTIFICATION_PURCHASE_SUCCEEDED_WITH_RECEIPT);
        }
 
        if (!NotificationCenter.Instance.HandlerSetHasObserver (billing_init_succeeded, bfgPurchaseAndroid.NOTIFICATION_BILLING_INITIALIZE_SUCCEEDED)) {
            NotificationCenter.Instance.AddObserver (billing_init_succeeded, bfgPurchaseAndroid.NOTIFICATION_BILLING_INITIALIZE_SUCCEEDED);
        }

        if (!NotificationCenter.Instance.HandlerSetHasObserver (billing_init_failed, bfgPurchaseAndroid.NOTIFICATION_BILLING_INITIALIZE_FAILED)) {
            NotificationCenter.Instance.AddObserver (billing_init_failed, bfgPurchaseAndroid.NOTIFICATION_BILLING_INITIALIZE_FAILED);
        }
    #endif
#else
        DOTween.Sequence()
               .InsertCallback(UnityEngine.Random.Range(0f, 3f),
                    () => { onInitComplete(IapErrorCode.NoError); });
#endif
    }

#if UNITY_ANDROID
    private void billing_init_failed(string arg)
    {
        Debug.Log($"BfgIapProvider: billing_init_failed: {arg}");
        onInitComplete.Invoke(IapErrorCode.InitFailReasonUnknown);
    }

    private void billing_init_succeeded(string arg)
    {
        Debug.Log($"BfgIapProvider: billing_init_succeeded: {arg}");
        onInitComplete.Invoke(IapErrorCode.NoError);
    }
    
    private void purchase_succeeded_with_receipt(string arg)
    {
        Debug.Log($"BfgIapProvider: purchase_succeeded_with_receipt: {arg}");
    }
#endif
    
    private string IdToStoreId(string id)
    {
        return IapCollection.GetStoreId(id);
    }
    
    private string StoreIdToId(string storeId)
    {
        return IapCollection.GetIdByStoreId(storeId);
    }
    
#region BFG methods

    private void OnRestoreFailedCallback(string productId)
    {
        remainingToRestore--;

        Debug.Log($"BfgIapProvider: OnRestoreFailedCallback: for '{productId}'");
        
        if (remainingToRestore <= 0)
        {
            OnRestoreCompleted?.Invoke(true);
        } 
    }

    private void OnRestoreSucceededCallback(string productId)
    {
        remainingToRestore--;
        
        Debug.Log($"BfgIapProvider: OnRestoreSucceededCallback: for '{productId}'");
        
        //OnPurchaseOK?.Invoke(StoreIdToId(productId), null, true);

        if (remainingToRestore <= 0)
        {
            OnRestoreCompleted?.Invoke(true);
        } 
    }

    private void OnPurchaseFailedCallback(string productId)
    {
        OnPurchaseFail?.Invoke(StoreIdToId(productId), IapErrorCode.PurchaseFailReasonUnknown);
    }

    private void OnPurchaseSuccessCallback(string productId, ProductInfo productinfo)
    {
        OnPurchaseOK?.Invoke(StoreIdToId(productId), null, false);
    }

#endregion    
    
    protected override void StartPurchase(string productId)
    {
#if !UNITY_EDITOR
        bool canPurchase = PurchaseController.Instance.Purchase(IdToStoreId(productId));
        if (!canPurchase)
        {
            OnPurchaseFail?.Invoke(productId, IapErrorCode.PurchaseFailReasonUnknown);
        }
#else
        DOTween.Sequence()
               .InsertCallback(UnityEngine.Random.Range(0f, 2f), () =>
                {
                    OnPurchaseOK?.Invoke(productId, null, false);
                });
        
        return;
#endif
    }
    
    public override void RestorePurchases()
    {
#if UNITY_IOS
        throw new NotImplementedException();
#endif
        
        if (remainingToRestore > 0)
        {
            Debug.LogWarning($"BfgIapProvider: RestorePurchases: Already in progress for {remainingToRestore} products");
            return;
        }

        remainingToRestore = IapCollection.GetConsumableCount();
        
        bfgPurchase.restorePurchases();
    }

    public override string GetLocalizedPriceStr(string productId)
    {
#if UNITY_EDITOR
        return null;
#endif
        
        string storeId = IapCollection.GetStoreId(productId);
        var product = PurchaseController.Instance.GetProductInfo(storeId);
        
        // Bfg sdk may return hardcoded english text when have no price
        if (product?.price == "Price Unavailable")
        {
            return null;
        }
        
        return product?.price;
    }

    public override void CleanUp()
    {
        if (PurchaseController.Instance != null)
        {
            PurchaseController.Instance.PurchaseSucceededEvent -= OnPurchaseSuccessCallback;
            PurchaseController.Instance.PurchaseFailedEvent -= OnPurchaseFailedCallback;
            PurchaseController.Instance.RestoreSucceededEvent -= OnRestoreSucceededCallback;
            PurchaseController.Instance.RestoreFailedEvent -= OnRestoreFailedCallback;
        }

        if (NotificationCenter.Instance != null)
        {
#if UNITY_ANDROID
            NotificationCenter.Instance.RemoveObserver(purchase_succeeded_with_receipt, bfgPurchaseAndroid.NOTIFICATION_PURCHASE_SUCCEEDED_WITH_RECEIPT);
            NotificationCenter.Instance.RemoveObserver(billing_init_succeeded, bfgPurchaseAndroid.NOTIFICATION_BILLING_INITIALIZE_SUCCEEDED);
            NotificationCenter.Instance.RemoveObserver(billing_init_failed,    bfgPurchaseAndroid.NOTIFICATION_BILLING_INITIALIZE_FAILED);
#endif
        }
    }
}

#endif