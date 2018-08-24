using System;
using UnityEngine;

public class EnergyCurrencyLogicComponent : LimitCurrencyLogicComponent, IECSSystem
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    public int Delay { get; set; }
    
    private DateTime then;
    public DateTime Later;
    
    public Action OnExecute;
    
    public long LastUpdate => then.ConvertToUnixTime();

    public override void OnRegisterEntity(ECSEntity entity)
    {
        targetItem = ProfileService.Current.Purchases.GetStorageItem(Currency.Energy.Name);
        limitItem = ProfileService.Current.Purchases.GetStorageItem(Currency.EnergyLimit.Name);
        
        then = DateTime.UtcNow;
        Later = then.AddSeconds(Delay);
        
        base.OnRegisterEntity(entity);
    }
    
    protected override void InitInSave()
    {
        var save = ProfileService.Current.GetComponent<CurrencySaveComponent>(CurrencySaveComponent.ComponentGuid);
        
        if(save == null) return;
        
        var refil = DateTimeExtension.CountOfStepsPassedWhenAppWasInBackground(save.EnergyLastUpdate, Delay, out then);
        
        Later = then.AddSeconds(Delay);
        targetItem.Amount += Mathf.Min(refil, limitItem.Amount - targetItem.Amount);
    }
    
    public void Execute()
    {
        OnExecute?.Invoke();
        
        var now = DateTime.UtcNow;
        
        if ((now - then).TotalSeconds < Delay) return;
        
        then = now;
        Later = then.AddSeconds(Delay);
        Add(1);
    }

    protected override void Add(int amount, bool isExtra = false)
    {
        base.Add(amount, isExtra);
        OnExecute?.Invoke();
    }

    public object GetDependency()
    {
        return null;
    }

    public bool IsExecuteable()
    {
        return targetItem.Amount < limitItem.Amount;
    }
}