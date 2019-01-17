using UnityEngine;

public class LimitCurrencyLogicComponent : ECSEntity
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    protected StorageItem targetItem;
    protected StorageItem limitItem;
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        base.OnRegisterEntity(entity);
        InitInSave();
    }

    public override void OnUnRegisterEntity(ECSEntity entity)
    {
        base.OnUnRegisterEntity(entity);
        targetItem = null;
        limitItem = null;
    }

    public virtual void InitInSave()
    {
    }
    
    protected virtual void Add(int amount)
    {
        if (amount <= 0) return;
        
        if (limitItem.Amount <= targetItem.Amount) return;

        var possible = Mathf.Min(amount, limitItem.Amount - targetItem.Amount);

        CurrencyHellper.Purchase(targetItem.Currency, possible);
    }
}