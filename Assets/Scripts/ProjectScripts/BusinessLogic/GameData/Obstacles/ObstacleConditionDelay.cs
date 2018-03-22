using System;

public class ObstacleConditionDelay : IObstacleCondition
{
    public int Delay { get; set; }

    public DateTime StartTime;
    public DateTime CompleteTime;

    public bool IsInitialized { get; set; }

    public void Init()
    {
        IsInitialized = true;
        StartTime = DateTime.Now;
        CompleteTime = StartTime.AddSeconds(Delay);
    }

    public bool Check(ObstaclesLogicComponent context)
    {
        if (IsInitialized == false) return false;
        
        return (DateTime.Now - StartTime).TotalSeconds >= Delay;
    }
}