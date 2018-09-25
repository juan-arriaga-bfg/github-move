using Newtonsoft.Json;
using Quests;

[JsonObject(MemberSerialization.OptIn)]
public abstract class TaskCounterEntity : TaskEntity
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
        if (IsCompleted())
        {
            return true;
        }

        if (IsInProgress())
        {
            bool result = currentValue > TargetValue;
            if (result)
            {
                State = TaskState.Completed;
                return true;
            }
        }

        return false;
    }

    public override string ToString()
    {
        string ret = $"Task: {Id}, State: {State}, Progress: {CurrentValue}/{TargetValue}";
        return ret;
    }
}