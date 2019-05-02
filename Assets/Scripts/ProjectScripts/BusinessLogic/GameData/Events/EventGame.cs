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

public class EventGame
{
    public EventGameType EventType;
    public EventGameState State;

    public List<EventGameStepDef> Steps;
    
    public int Step => ProfileService.Current.GetStorageItem(Currency.EventStep.Name).Amount;
    
    public int Price
    {
        get
        {
            var step = IsCompleted ? Step - 1 : Step;
            return Steps[step].Prices[0].Amount;
        }
    }

    public bool IsActive => IsCompleted == false;

    public bool IsPremium => false;
    
    public bool IsLastStep => Step == Steps.Count - 1;
    
    public bool IsCompleted => Step == Steps.Count;
}