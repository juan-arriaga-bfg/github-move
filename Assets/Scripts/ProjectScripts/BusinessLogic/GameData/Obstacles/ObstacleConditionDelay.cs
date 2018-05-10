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
        StartTime = DateTime.UtcNow;
        CompleteTime = StartTime.AddSeconds(Delay);
    }

    public bool Check(ObstaclesLogicComponent context)
    {
        if (IsInitialized == false) return false;
        
        return (DateTime.UtcNow - StartTime).TotalSeconds >= Delay;
    }
}