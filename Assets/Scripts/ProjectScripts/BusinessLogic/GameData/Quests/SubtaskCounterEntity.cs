using Newtonsoft.Json;
using Quests;

[JsonObject(MemberSerialization.OptIn)]
public abstract class SubtaskCounterEntity : SubtaskEntity
{
    [JsonProperty] 
    public int TargetValue  { get; protected set; }
    
    [JsonProperty(PropertyName = "CurrentValue")] 
    private int currentValue;
    
    public int CurrentValue
    {
        get { return currentValue; }
        set
        {
            if (currentValue == value)
            {
                return;
            }
            
            currentValue = value;
            Check();
            OnChanged(this);
        }
    }

    protected override bool Check()
    {
        switch (State)
        {
            case SubtaskState.Completed:
                return true;
            
            case SubtaskState.InProgress:
            {
                bool result = currentValue > TargetValue;
                if (result)
                {
                    State = SubtaskState.Completed;
                    return true;
                }

                break;
            }
        }

        return false;
    }
}