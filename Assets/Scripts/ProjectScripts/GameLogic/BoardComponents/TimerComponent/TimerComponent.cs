using System;
using UnityEngine;

public class TimerComponent : IECSComponent, IECSSystem
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;

    public int Delay;
    public CurrencyPair Price;
    
    public Action OnStart;
    public Action OnExecute;
    public Action OnComplete;
    public Action OnStop;
    
    public DateTime StartTime;
    public DateTime CompleteTime;
    
    public long StartTimeLong => StartTime.ConvertToUnixTime();
    
    public bool IsStarted { get; set; }
    public bool IsPaused { get; set; }
    
    public void OnRegisterEntity(ECSEntity entity)
    {
    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }
    
    public void Start()
    {
        Start(DateTime.UtcNow);
    }

    public void Start(long start)
    {
        Start(DateTimeExtension.UnixTimeToDateTime(start));
    }
    
    public void Start(DateTime start)
    {
        IsStarted = true;
        IsPaused = false;
        StartTime = start;
        CompleteTime = StartTime.AddSeconds(Delay);
        OnStart?.Invoke();
    }
    
    public void Stop()
    {
        IsStarted = false;
        OnStop?.Invoke();
    }
    
    public void Execute()
    {
        OnExecute?.Invoke();
        
        if(StartTime.GetTime().TotalSeconds < Delay) return;

        IsStarted = false;

        OnComplete?.Invoke();
    }
    
    public bool IsExecuteable()
    {
        return IsPaused == false && IsStarted;
    }
    
    public object GetDependency()
    {
        return null;
    }
    
    public float GetProgress()
    {
        return (int)StartTime.GetTime().TotalSeconds / (float)Delay;
    }
    
    public CurrencyPair GetPrise()
    {
        if (Price == null) return null;

        var amount = Mathf.Max(1, (int) (Price.Amount * (1 - GetProgress())));
        
        return  new CurrencyPair {Currency = Price.Currency, Amount = amount};
    }
}