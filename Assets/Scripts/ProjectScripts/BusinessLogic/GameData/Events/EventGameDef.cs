using System.Collections.Generic;

public enum EventGameType
{
    OrderSoftLaunch,
}

public enum EventGameState
{
    Default,
    Preview,
    InProgress,
    Complete,
    Claimed
}

public class EventGameDef
{
    public EventGameType EventType;
    public EventGameState State;

    public List<EventGameStepDef> Steps;
}