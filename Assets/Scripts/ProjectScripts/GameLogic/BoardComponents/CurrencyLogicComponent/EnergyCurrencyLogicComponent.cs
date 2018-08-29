using System;
using UnityEngine;

public class EnergyCurrencyLogicComponent : LimitCurrencyLogicComponent, IECSSystem
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    private TimerComponent timer;
    public TimerComponent Timer
    {
        get { return timer ?? (timer = GetComponent<TimerComponent>(TimerComponent.ComponentGuid)); }
    }
    
    public int Delay { get; set; }
    
//    private DateTime then;
//    public DateTime Later;
//    
    public Action OnExecute;
    
    public long LastUpdate => Timer.StartTime.ConvertToUnixTime();

    public override void OnRegisterEntity(ECSEntity entity)
    {
        targetItem = ProfileService.Current.Purchases.GetStorageItem(Currency.Energy.Name);
        limitItem = ProfileService.Current.Purchases.GetStorageItem(Currency.EnergyLimit.Name);

        Timer.Delay = Delay;
        Timer.OnComplete += StepComplete; 
        
        base.OnRegisterEntity(entity);
    }
    
    protected override void InitInSave()
    {
        var save = ProfileService.Current.GetComponent<CurrencySaveComponent>(CurrencySaveComponent.ComponentGuid);
        
        if(save == null) return;
        
        var refil = DateTimeExtension.CountOfStepsPassedWhenAppWasInBackground(save.EnergyLastUpdate, Delay, out Timer.StartTime);
        
        targetItem.Amount += Mathf.Min(refil, limitItem.Amount - targetItem.Amount);
        
        if(IsExecuteable())
            Execute();
    }
    
    public void Execute()
    {
        OnExecute?.Invoke();

        if (Timer.IsStarted == false && targetItem.Amount < limitItem.Amount)
        {
            Timer.Start();
        }
    }

    public void StepComplete()
    {
        Add(1);
        if(targetItem.Amount >= limitItem.Amount)
            Timer.Stop();
        else
            Timer.Start();
        
        Execute();
    }

    protected override void Add(int amount, bool isExtra = false)
    {
        base.Add(amount, isExtra);
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