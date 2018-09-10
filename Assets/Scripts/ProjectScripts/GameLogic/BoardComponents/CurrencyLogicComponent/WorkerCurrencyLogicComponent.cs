using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

public class WorkerCurrencyLogicComponent : LimitCurrencyLogicComponent, IECSSystem
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    private Dictionary<string, DateTime> completeTimes = new Dictionary<string, DateTime>();
    private readonly List<string> completeTimesList = new List<string>();

    private DateTime then;
    private BoardController context;
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        context = entity as BoardController;
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
            completeTimesList.Remove(key);
        }
        
        Add(remove.Count);
    }

    public object GetDependency()
    {
        return null;
    }

    public bool Get(string id, int delay)
    {
        if (CurrencyHellper.IsCanPurchase(targetItem.Currency, 1) == false)
        {
            var str = completeTimesList[Random.Range(0, completeTimesList.Count)];
            var position = BoardPosition.Parse(str);
            
            UIErrorWindowController.AddError("All workers are busy!");
            context.HintCooldown.Step(position);
            return false;
        }
        
        var isSuccess = false;
        
        CurrencyHellper.Purchase(Currency.Mine.Name, 1, targetItem.Currency, 1, success =>
        {
            isSuccess = success;

            if (isSuccess == false) return;
            
            completeTimes.Add(id, DateTime.UtcNow.AddSeconds(delay));
            completeTimesList.Add(id);
        });
        
        return isSuccess;
    }

    public bool Replase(string oldKey, string newKey)
    {
        DateTime value;
        if(completeTimes.TryGetValue(oldKey, out value) == false) return false;
        
        completeTimes.Remove(oldKey);
        completeTimesList.Remove(oldKey);
        
        completeTimes.Add(newKey, value);
        completeTimesList.Add(newKey);
        
        return true;
    }
    
    public bool Return(string id)
    {
        if(completeTimes.ContainsKey(id) == false) return false;
        
        completeTimes.Remove(id);
        completeTimesList.Remove(id);
        Add(1);
        
        return true;
    }
    
    public bool IsExecuteable()
    {
        return targetItem.Amount < limitItem.Amount;
    }
}