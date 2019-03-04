using System;
using UnityEngine;

public class EnergyCurrencyLogicComponent : LimitCurrencyLogicComponent, IECSSystem
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    private TimerComponent timer;
    public TimerComponent Timer => timer ?? (timer = GetComponent<TimerComponent>(TimerComponent.ComponentGuid));

    private int delay => GameDataService.Current.ConstantsManager.EnergyRefillDelay; 
    
    public Action OnExecute;
    
    public long LastUpdate => Timer.StartTime.ConvertToUnixTime();

    public override void OnRegisterEntity(ECSEntity entity)
    {
        RegisterComponent(new TimerComponent());
        
        targetItem = ProfileService.Current.Purchases.GetStorageItem(Currency.Energy.Name);
        limitItem = ProfileService.Current.Purchases.GetStorageItem(Currency.EnergyLimit.Name);
        
        LocalNotificationsService.Current.RegisterNotifier(new Notifier(Timer, NotifyType.EnergyRefresh));
        
        Timer.Delay = delay;
        Timer.OnComplete += StepComplete;
        
        // Timer.StartTime = DateTime.Now;
        var secureTime = SecuredTimeService.Current;
        Timer.StartTime = secureTime.Now;
        Timer.OnTimeChanged += TimerStopCheck;
        
        base.OnRegisterEntity(entity);
    }

    public override void OnUnRegisterEntity(ECSEntity entity)
    {       
        Timer.OnTimeChanged -= TimerStopCheck;
        Timer.OnComplete -= StepComplete;
        
        base.OnUnRegisterEntity(entity);
    }

    private void TimerStopCheck()
    {
        if(!CheckIsNeed() && Timer.IsExecuteable()) Timer.Stop();
    }
    
    public override void InitInSave()
    {
        if(CheckIsNeed() == false) return;
        
        var save = ProfileService.Current.GetComponent<CurrencySaveComponent>(CurrencySaveComponent.ComponentGuid);
        
        if(save == null) return;

        DateTime startTime;
        var refill = DateTimeExtension.CountOfStepsPassedWhenAppWasInBackground(save.EnergyLastUpdate, delay, out startTime);
        Timer.StartTime = startTime;
        
        Add(Mathf.Min(refill, limitItem.Amount - targetItem.Amount));

        if (CheckIsNeed())
        {
            Timer.Start(Timer.StartTime);
            return;
        }
        
        OnExecute?.Invoke();
    }

    public bool CheckIsNeed()
    {
        return targetItem.Amount < limitItem.Amount;
    }
    
    public void Execute()
    {
        if (!Timer.IsStarted && CheckIsNeed()) Timer.Start();
        
        OnExecute?.Invoke();
    }

    public void StepComplete()
    {
        Add(1);
        
        if(targetItem.Amount >= limitItem.Amount) Timer.Stop();
        else Timer.Start();
        
        Execute();
    }

    public object GetDependency()
    {
        return null;
    }

    public bool IsExecuteable()
    {
        return CheckIsNeed();
    }
}