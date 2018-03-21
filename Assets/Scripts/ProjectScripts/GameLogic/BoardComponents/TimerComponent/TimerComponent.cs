using System;

public class TimerComponent : IECSComponent, IECSSystem
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    
    public int Guid { get { return ComponentGuid; } }
    
    public int Delay;
    public Action OnComplete;
    
    private DateTime startTime;

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
        IsStarted = true;
        IsPaused = false;
        startTime = DateTime.Now;
    }
    
    public void Stop()
    {
        IsStarted = false;
    }
    
    public bool IsExecuteable()
    {
        return IsPaused == false && IsStarted;
    }

    public void Execute()
    {
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

    public string GetTimeText(string format)
    {
        var time = GetTime();
        
        if (string.IsNullOrEmpty(format))
        {
            return (int) time.TotalHours > 0 ? string.Format("{0:00}:{1:00}", time.Hours, time.Minutes) : string.Format("{0:00}:{1:00}", time.Minutes, time.Seconds);
        }
        
        return string.Format(format, time.Hours, time.Minutes, time.Seconds);
    }
}