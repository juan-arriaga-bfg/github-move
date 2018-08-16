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
    
    private DateTime startTime;
    private DateTime completeTime;

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
        startTime = start;
        completeTime = startTime.AddSeconds(Delay);
        OnStart?.Invoke();
    }
    
    public void Stop()
    {
        IsStarted = false;
        OnStop?.Invoke();
    }
    
    public bool IsExecuteable()
    {
        return IsPaused == false && IsStarted;
    }

    public void Execute()
    {
        OnExecute?.Invoke();

        var time = GetTime();
        
        if(time.TotalSeconds < Delay) return;

        IsStarted = false;

        OnComplete?.Invoke();
    }

    public object GetDependency()
    {
        return null;
    }
    
    public long StartTime => startTime.ConvertToUnixTime();

    public TimeSpan GetTime()
    {
        return DateTime.UtcNow - startTime;
    }

    public TimeSpan GetTimeLeft()
    {
        return completeTime - DateTime.UtcNow;
    }

    public string GetDelayText(string format)
    {
        return TimeFormat(DateTime.UtcNow.AddSeconds(Delay) - DateTime.UtcNow, format);
    }

    public string GetTimeText(string format)
    {
        return TimeFormat(GetTime(), format);
    }
    
    public string GetTimeLeftText(string format)
    {
        return TimeFormat(GetTimeLeft(), format);
    }

    public float GetProgress()
    {
        return (int)GetTime().TotalSeconds / (float)Delay;
    }
    
    public CurrencyPair GetPrise()
    {
        if (Price == null) return null;

        var amount = Mathf.Max(1, (int) (Price.Amount * (1 - GetProgress())));
        
        return  new CurrencyPair {Currency = Price.Currency, Amount = amount};
    }
    
    private string TimeFormat(TimeSpan time, string format)
    {
        if (string.IsNullOrEmpty(format))
        {
            return (int) time.TotalHours > 0
                ? $"<mspace=3em>{time.Hours:00}:{time.Minutes:00}:{time.Seconds:00}</mspace>"
                : $"<mspace=3em>{time.Minutes:00}:{time.Seconds:00}</mspace>";
        }
        
        return string.Format(format, time.Hours, time.Minutes, time.Seconds);
    }
}