using System;

public class NowTimer: ITimerComponent
{
    public DateTime StartTime { get; }
    public DateTime CompleteTime => UseUTC ? DateTime.UtcNow : DateTime.Now;

    public bool IsExecuteable() { return true; }

    public bool UseUTC { get; set; } = true;
}
