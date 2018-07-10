using UnityEngine;

public class LimitCurrencyLogicComponent : IECSComponent
{
    public virtual int Guid { get { return 0; } }

    protected StorageItem targetItem;
    protected StorageItem limitItem;
    
    public virtual void OnRegisterEntity(ECSEntity entity)
    {
    }

    public virtual void OnUnRegisterEntity(ECSEntity entity)
    {
        targetItem = null;
        limitItem = null;
    }
    
    public virtual void UpLimit(int amount)
    {
        if (amount <= 0) return;
        
        CurrencyHellper.Purchase(limitItem.Currency, amount);
    }

    public virtual bool Add(int amount, bool isExtra = false)
    {
        if (amount <= 0) return false;
        
        if (isExtra)
        {
            CurrencyHellper.Purchase(targetItem.Currency, amount);
            return true;
        }
        
        if (limitItem.Amount <= targetItem.Amount) return false;

        var possible = Mathf.Min(amount, limitItem.Amount - targetItem.Amount);

        CurrencyHellper.Purchase(targetItem.Currency, possible);

        return true;
    }
}