using System;
using System.Collections.Generic;

public class WorkerCurrencyLogicComponent : LimitCurrencyLogicComponent, IECSSystem
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid { get { return ComponentGuid; } }
    
    private readonly Dictionary<string, DateTime> completeTimes = new Dictionary<string, DateTime>();

    private DateTime then;
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        targetItem = ProfileService.Current.Purchases.GetStorageItem(Currency.Worker.Name);
        limitItem = ProfileService.Current.Purchases.GetStorageItem(Currency.WorkerLimit.Name);
        
        then = DateTime.UtcNow;
    }
    
    public void Execute()
    {
        var now = DateTime.UtcNow;
        
        if ((now - then).TotalSeconds < 1) return;
        
        then = now;
        
        var remove = new List<string>();

        foreach (var pair in completeTimes)
        {
            if((pair.Value - now).TotalSeconds > 1) continue;
            
            remove.Add(pair.Key);
        }
        
        foreach (var key in remove)
        {
            completeTimes.Remove(key);
            Add(1);
        }
    }

    public bool Get(string id, int delay)
    {
        var isSuccess = false;
        
        CurrencyHellper.Purchase(Currency.Mine.Name, 1, targetItem.Currency, 1, success =>
        {
            isSuccess = success;

            if (isSuccess == false) return;
            
            completeTimes.Add(id, DateTime.UtcNow.AddSeconds(delay));
        });
        
        return isSuccess;
    }
    
    public void Return(string id)
    {
        if(completeTimes.ContainsKey(id) == false) return;
        
        completeTimes.Remove(id);
        Add(1);
    }
    
    public bool IsExecuteable()
    {
        return targetItem.Amount < limitItem.Amount;
    }
    
    public bool IsPersistence { get{ return false; } }
}