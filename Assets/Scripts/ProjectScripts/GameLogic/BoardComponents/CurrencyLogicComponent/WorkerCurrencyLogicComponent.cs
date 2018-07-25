using System;
using System.Collections.Generic;
using System.Text;

public class WorkerCurrencyLogicComponent : LimitCurrencyLogicComponent, IECSSystem
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid { get { return ComponentGuid; } }

    private Dictionary<string, DateTime> completeTimes = new Dictionary<string, DateTime>();

    private DateTime then;
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        targetItem = ProfileService.Current.Purchases.GetStorageItem(Currency.Worker.Name);
        limitItem = ProfileService.Current.Purchases.GetStorageItem(Currency.WorkerLimit.Name);
        
        then = DateTime.UtcNow;
        
        base.OnRegisterEntity(entity);
    }

    protected override void InitInSave()
    {
        var save = ProfileService.Current.GetComponent<CurrencySaveComponent>(CurrencySaveComponent.ComponentGuid);
        
        if(save == null) return;

        completeTimes = new Dictionary<string, DateTime>();
        
        if(string.IsNullOrEmpty(save.WorkerUnlockDelay)) return;
        
        var workers = save.WorkerUnlockDelay.Split(new[] {";"}, StringSplitOptions.RemoveEmptyEntries);

        foreach (var worker in workers)
        {
            var arr = worker.Split(new[] {":"}, StringSplitOptions.RemoveEmptyEntries);
            
            completeTimes.Add(arr[0], DateTimeExtension.UnixTimeToDateTime(long.Parse(arr[1])));
        }
    }

    public string Save()
    {
        var str = new StringBuilder();
        
        if(completeTimes.Count == 0) return string.Empty;

        foreach (var completeTime in completeTimes)
        {
            str.AppendFormat("{0}:{1};", completeTime.Key, completeTime.Value.ConvertToUnixTime());
        }

        return str.ToString();
    }

    public void Execute()
    {
        var now = DateTime.UtcNow;
        
        if ((now - then).TotalSeconds < 1) return;
        
        then = now;
        
        var remove = new List<string>();
        
        foreach (var pair in completeTimes)
        {
            if((pair.Value - now).TotalSeconds > 0.5) continue;
            
            remove.Add(pair.Key);
        }
        
        if(remove.Count == 0) return;
        
        foreach (var key in remove)
        {
            completeTimes.Remove(key);
        }
        
        Add(remove.Count);
    }

    public object GetDependency()
    {
        return null;
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
}