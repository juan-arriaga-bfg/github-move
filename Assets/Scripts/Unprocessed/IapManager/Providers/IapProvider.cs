using System;

public abstract class IapProvider
{
    public IapCollection IapCollection { get; protected set; }
    
    public IapProvider SetIapCollection(IapCollection iapCollection)
    {
        IapCollection = iapCollection;
        return this;
    }
    
    public delegate void OnPurchaseOkDelegate(string productId, string receipt);
    public OnPurchaseOkDelegate OnPurchaseOK;

    public delegate void OnPurchaseFailDelegate(string productId, IapErrorCode errorCode);
    public OnPurchaseFailDelegate OnPurchaseFail;
    
    public Action<bool> OnRestoreCompleted;
    
    public abstract void Init(Action<IapErrorCode> onComplete);

    protected abstract void StartPurchase(string productId);
    
    public abstract void RestorePurchases();
    
    public abstract string GetLocalizedPriceStr(string productId);
    
    public void Purchase(string id)
    {
        StartPurchase(id);
    }

    /// <summary>
    /// Clean all before Destroy: listners, statics, singltons, etc
    /// </summary>
    public abstract void CleanUp();
}