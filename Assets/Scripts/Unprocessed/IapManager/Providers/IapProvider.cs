using System;
using UnityEngine;

public abstract class IapProvider
{
    public IapCollection IapCollection { get; protected set; }
    
    public IapProvider SetIapCollection(IapCollection iapCollection)
    {
        IapCollection = iapCollection;
        return this;
    }
    
    public delegate void OnPurchaseOkDelegate(string productId, string receipt, bool restore);
    public OnPurchaseOkDelegate OnPurchaseOK;

    public delegate void OnPurchaseFailDelegate(string productId, IapErrorCode errorCode);
    public OnPurchaseFailDelegate OnPurchaseFail;
    
    public Action<bool> OnRestoreCompleted;
    
    public abstract void Init(Action<IapErrorCode> onComplete);

    protected abstract void StartPurchase(string productId);
    
    public abstract void RestorePurchases();
    
    public abstract string GetLocalizedPriceStr(string productId);
    
    public abstract long GetPriceAsNumber(string productId);
    
    public void Purchase(string id)
    {
        if (string.IsNullOrEmpty(id) || !IapCollection.InapIds.Contains(id))
        {
            Debug.LogError($"[IapProvider] => Purchase id: '{id ?? "null"}' not defined in IapCollection");
            OnPurchaseFail?.Invoke(id, IapErrorCode.PurchaseFailNoProductWithIdDefined);
            return;
        }
        
        StartPurchase(id);
    }

    /// <summary>
    /// Clean all before Destroy: listners, statics, singltons, etc
    /// </summary>
    public abstract void CleanUp();
}