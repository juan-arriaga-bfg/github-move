﻿using System;

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
    
    public long StartTime
    {
        get { return startTime.ConvertToUnixTime(); }
    }
    
    public TimeSpan GetTime()
    {
        return DateTime.UtcNow - startTime;
    }

    public TimeSpan GetTimeLeft()
    {
        return completeTime - DateTime.UtcNow;
    }

    public string GetTimeText(string format)
    {
        return TimeFormat(GetTime(), format);
    }
    
    public string GetTimeLeftText(string format)
    {
        return TimeFormat(GetTimeLeft(), format);
    }

    public int CountOfStepsPassedWhenAppWasInBackground(long then, out long now)
    {
        DateTime nowTime;
        var count = CountOfStepsPassedWhenAppWasInBackground(DateTimeExtension.UnixTimeToDateTime(then), out nowTime);
        
        now = nowTime.ConvertToUnixTime();
        return count;
    }
    
    public int CountOfStepsPassedWhenAppWasInBackground(DateTime then, out DateTime now)
    {
        var elapsedTime = DateTime.UtcNow - then;
        var count = (int) elapsedTime.TotalSeconds / Delay;
        var remainder = (int) elapsedTime.TotalSeconds % Delay;
        
        now = DateTime.UtcNow.AddSeconds(-remainder);
        
        return count;
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