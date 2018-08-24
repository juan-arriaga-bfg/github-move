using UnityEngine;

public class LimitCurrencyLogicComponent : IECSComponent
{
    public virtual int Guid => 0;

    protected StorageItem targetItem;
    protected StorageItem limitItem;
    
    public virtual void OnRegisterEntity(ECSEntity entity)
    {
        InitInSave();
    }

    public virtual void OnUnRegisterEntity(ECSEntity entity)
    {
        targetItem = null;
        limitItem = null;
    }

    protected virtual void InitInSave()
    {
    }
    
    protected virtual void Add(int amount, bool isExtra = false)
    {
        if (amount <= 0) return;
        
        if (isExtra)
        {
            CurrencyHellper.Purchase(targetItem.Currency, amount);
            return;
        }
        
        if (limitItem.Amount <= targetItem.Amount) return;

        var possible = Mathf.Min(amount, limitItem.Amount - targetItem.Amount);

        CurrencyHellper.Purchase(targetItem.Currency, possible);
    }
}