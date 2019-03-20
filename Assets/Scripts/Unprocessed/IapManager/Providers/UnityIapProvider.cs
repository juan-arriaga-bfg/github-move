#if UNITY_PURCHASES
using Debug = IW.Logger;
using System;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;

public class UnityIapProvider : IapProvider, IStoreListener
{
    private IStoreController m_Controller;

    private IAppleExtensions m_AppleExtensions;
    private ITransactionHistoryExtensions m_TransactionHistoryExtensions;
    
    private bool m_IsGooglePlayStoreSelected;
    private bool m_PurchaseInProgress;
    private string m_LastTransactionID;

    private Action<IapErrorCode> OnInitCompleted;
    
    public override void Init(Action<IapErrorCode> onComplete)
    {
        OnInitCompleted = onComplete;
        
        var module = StandardPurchasingModule.Instance();

        // The FakeStore supports: no-ui (always succeeding), basic ui (purchase pass/fail), and
        // developer ui (initialization, purchase, failure code setting). These correspond to
        // the FakeStoreUIMode Enum values passed into StandardPurchasingModule.useFakeStoreUIMode.
        module.useFakeStoreUIMode = FakeStoreUIMode.DeveloperUser;

        var builder = ConfigurationBuilder.Instance(module);

        // Set this to true to enable the Microsoft IAP simulator for local testing.
        //builder.Configure<IMicrosoftConfiguration>().useMockBillingSystem = false;

        m_IsGooglePlayStoreSelected = Application.platform == RuntimePlatform.Android && module.appStore == AppStore.GooglePlay;

        foreach (var item in IapCollection.Defs)
        {
            var productType = item.Consumable ? ProductType.Consumable : ProductType.NonConsumable;
            builder.AddProduct(item.Id, productType, new IDs
            {
                {item.AmazonAppStoreId, AmazonApps.Name},
                {item.GooglePlayId, GooglePlay.Name},
                {item.AppleAppStoreId, AppleAppStore.Name}
            });
        }
        
        // Write Amazon's JSON description of our products to storage when using Amazon's local sandbox.
        // This should be removed from a production build.
#if DEBUG && AMAZON
        builder.Configure<IAmazonConfiguration>().WriteSandboxJSON(builder.products);
#endif

#if RECEIPT_VALIDATION
        string appIdentifier;
        #if UNITY_5_6_OR_NEWER
        appIdentifier = Application.identifier;
        #else
        appIdentifier = Application.bundleIdentifier;
        #endif
        validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(),
            UnityChannelTangle.Data(), appIdentifier);
#endif

        UnityPurchasing.Initialize(this, builder);
    }

    protected override void StartPurchase(string id)
    {
        CrossPlatformValidator v = new CrossPlatformValidator(null, null,null); 
        
        if (m_PurchaseInProgress)
        {
            OnPurchaseFail?.Invoke(id, IapErrorCode.PurchaseFailSomePurchaseAlreadyInProgress);
            return;
        }
        
        if (m_Controller == null)
        {
            OnPurchaseFail?.Invoke(id, IapErrorCode.PurchaseFailIapPrviderNotInitialized);
            return;
        }
        
        if (m_Controller.products.WithID(id) == null)
        {
            Debug.LogError($"No product with id {id} is in the IapCollection");
            OnPurchaseFail?.Invoke(id, IapErrorCode.PurchaseFailNoProductWithIdDefined);
            return;
        }

        m_PurchaseInProgress = true;
        m_Controller.InitiatePurchase(m_Controller.products.WithID(id), "Payload");
    }

    public override void RestorePurchases()
    {
#if !UNITY_IOS
        throw new NotImplementedException();
#endif
        m_AppleExtensions.RestoreTransactions(OnTransactionsRestored);
    }

    public override string GetLocalizedPriceStr(string productId)
    {
        var ret = m_Controller?.products?.WithID(productId)?.metadata?.localizedPriceString;
        return ret;
    }

    public override void CleanUp()
    {
        // todo: do we need to cleanup something there?
    }

    private void OnTransactionsRestored(bool success)
    {
        OnRestoreCompleted?.Invoke(success);
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        IapErrorCode code = (IapErrorCode) ((int) error + 1000);
        OnInitCompleted?.Invoke(code);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
        Debug.Log("Purchase OK: " + e.purchasedProduct.definition.id);
        Debug.Log("Receipt: " + e.purchasedProduct.receipt);

        m_LastTransactionID = e.purchasedProduct.transactionID;
        m_PurchaseInProgress = false;

        OnPurchaseOK?.Invoke(e.purchasedProduct.definition.id, e.purchasedProduct.receipt);
        
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
    {
        Debug.Log($"Purchase failed: {product.definition.id}, reason: {reason}");

        // Detailed debugging information
        Debug.Log("Store specific error code: " + m_TransactionHistoryExtensions.GetLastStoreSpecificPurchaseErrorCode());
        if (m_TransactionHistoryExtensions.GetLastPurchaseFailureDescription() != null)
        {
            Debug.Log("Purchase failure description message: " +
                      m_TransactionHistoryExtensions.GetLastPurchaseFailureDescription().message);
        }

        m_PurchaseInProgress = false;
        
        IapErrorCode code = (IapErrorCode) ((int) reason + 2000); 
        
        OnPurchaseFail?.Invoke(product.definition.id, code);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        m_Controller = controller;
        m_AppleExtensions = extensions.GetExtension<IAppleExtensions>();
        m_TransactionHistoryExtensions = extensions.GetExtension<ITransactionHistoryExtensions>();

        // InitUI(controller.products.all);//keht

        // On Apple platforms we need to handle deferred purchases caused by Apple's Ask to Buy feature.
        // On non-Apple platforms this will have no effect; OnDeferred will never be called.
        m_AppleExtensions.RegisterPurchaseDeferredListener(OnDeferred);

        // This extension function returns a dictionary of the products' skuDetails from GooglePlay Store
        // Key is product Id (Sku), value is the skuDetails json string
        //Dictionary<string, string> google_play_store_product_SKUDetails_json = m_GooglePlayStoreExtensions.GetProductJSONDictionary();

        Debug.Log("Available items:");
        foreach (var item in controller.products.all)
        {
            if (item.availableToPurchase)
            {
                Debug.Log(string.Join(" | ",
                    new[]
                    {
                        item.metadata.localizedTitle,
                        item.metadata.localizedDescription,
                        item.metadata.isoCurrencyCode,
                        item.metadata.localizedPrice.ToString(),
                        item.metadata.localizedPriceString,
                        item.transactionID,
                        item.receipt
                    }));
            }
        }

        // Populate the product menu now that we have Products
        //AddProductUIs(m_Controller.products.all);// keht

        LogProductDefinitions();
        
        
        OnInitCompleted?.Invoke(IapErrorCode.NoError);
    }
    
    /// <summary>
    /// iOS Specific.
    /// This is called as part of Apple's 'Ask to buy' functionality,
    /// when a purchase is requested by a minor and referred to a parent
    /// for approval.
    ///
    /// When the purchase is approved or rejected, the normal purchase events
    /// will fire.
    /// </summary>
    /// <param name="item">Item.</param>
    private void OnDeferred(Product item)
    {
        Debug.Log("Purchase deferred: " + item.definition.id);
    }
    
    private void LogProductDefinitions()
    {
        var products = m_Controller.products.all;
        foreach (var product in products)
        {
            Debug.Log($"id: {product.definition.id}\nstore-specific id: {product.definition.storeSpecificId}\ntype: {product.definition.type.ToString()}\nenabled: {(product.definition.enabled ? "enabled" : "disabled")}\n");
        }
    }
}

#endif