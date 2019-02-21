using System;

public interface ITimerComponent
{
    DateTime StartTime { get; }
    DateTime CompleteTime { get; }
    
    bool IsExecuteable();

    bool UseUTC { get; set; }
}