using System;
using UnityEngine;

public class EnergyCurrencyLogicComponent : LimitCurrencyLogicComponent, IECSSystem
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid { get { return ComponentGuid; } }
    
    public int Delay { get; set; }
    
    private DateTime then;

    public long LastUpdate
    {
        get { return then.ConvertToUnixTime(); }
    }

    public override void OnRegisterEntity(ECSEntity entity)
    {
        targetItem = ProfileService.Current.Purchases.GetStorageItem(Currency.Energy.Name);
        limitItem = ProfileService.Current.Purchases.GetStorageItem(Currency.EnergyLimit.Name);
        
        then = DateTime.UtcNow;
        
        base.OnRegisterEntity(entity);
    }
    
    protected override void InitInSave()
    {
        var save = ProfileService.Current.GetComponent<CurrencySaveComponent>(CurrencySaveComponent.ComponentGuid);
        
        if(save == null) return;
        
        var refil = DateTimeExtension.CountOfStepsPassedWhenAppWasInBackground(save.EnergyLastUpdate, Delay, out then);
        
        targetItem.Amount += Mathf.Min(refil, limitItem.Amount - targetItem.Amount);
    }

    public void Execute()
    {
        var now = DateTime.UtcNow;
        
        if ((now - then).TotalSeconds < Delay) return;
        
        then = now;
        Add(1);
    }
    
    public bool IsExecuteable()
    {
        return targetItem.Amount < limitItem.Amount;
    }
    
    public bool IsPersistence { get{ return false; } }
}