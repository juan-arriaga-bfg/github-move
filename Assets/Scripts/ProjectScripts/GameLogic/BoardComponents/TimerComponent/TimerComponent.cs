using System;

public class TimerComponent : IECSComponent, IECSSystem
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    
    public int Guid { get { return ComponentGuid; } }
    
    public int Delay;
    
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

    public void Start(DateTime start)
    {
        IsStarted = true;
        IsPaused = false;
        startTime = start;
        completeTime = startTime.AddSeconds(Delay);
        if(OnStart != null) OnStart();
    }
    
    public void Start()
    {
        IsStarted = true;
        IsPaused = false;
        startTime = DateTime.Now;
        completeTime = startTime.AddSeconds(Delay);
        if(OnStart != null) OnStart();
    }
    
    public void Stop()
    {
        IsStarted = false;
        if(OnStop != null) OnStop();
    }
    
    public bool IsExecuteable()
    {
        return IsPaused == false && IsStarted;
    }

    public void Execute()
    {
        if (OnExecute != null) OnExecute();
        
        var time = GetTime();
        
        if(time.TotalSeconds < Delay) return;

        IsStarted = false;
        
        if(OnComplete != null) OnComplete();
    }

    public bool IsPersistence
    {
        get { return false; }
    }
    
    public TimeSpan GetTime()
    {
        return DateTime.Now - startTime;
    }

    public TimeSpan GetTimeLeft()
    {
        return completeTime - DateTime.Now;
    }

    public string GetTimeText(string format)
    {
        return TimeFormat(GetTime(), format);
    }
    
    public string GetTimeLeftText(string format)
    {
        return TimeFormat(GetTimeLeft(), format);
    }

    private string TimeFormat(TimeSpan time, string format)
    {
        if (string.IsNullOrEmpty(format))
        {
            return (int) time.TotalHours > 0
                ? string.Format("<mspace=3em>{0:00}:{1:00}:{2:00}</mspace>", time.Hours, time.Minutes, time.Seconds)
                : string.Format("<mspace=3em>{0:00}:{1:00}</mspace>", time.Minutes, time.Seconds);
        }
        
        return string.Format(format, time.Hours, time.Minutes, time.Seconds);
    }
    
}