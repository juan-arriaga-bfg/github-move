﻿using System;

public class EnergyCurrencyLogicComponent : LimitCurrencyLogicComponent, IECSSystem
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid { get { return ComponentGuid; } }
    
    public int Delay { get; set; }
    
    private DateTime then;

    public override void OnRegisterEntity(ECSEntity entity)
    {
        targetItem = ProfileService.Current.Purchases.GetStorageItem(Currency.Energy.Name);
        limitItem = ProfileService.Current.Purchases.GetStorageItem(Currency.EnergyLimit.Name);
        
        then = DateTime.UtcNow;
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