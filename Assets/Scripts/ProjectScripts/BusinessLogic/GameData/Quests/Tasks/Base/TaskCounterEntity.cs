using System;
using Newtonsoft.Json;
using Quests;

public enum CompareOperatorType
{
    GreaterOrEqual,   // >=
    LessOrEqual,      // <=
    GreaterThan,      // >
    LessThan,         // <
    Equal,            // ==
    NotEqual          // !=
}


/// <summary>
/// Base class for tasks that have counter - 'Current' of 'Target'. Current and Target values will be compared using specified CompareOperatorType. 
/// </summary>
[JsonObject(MemberSerialization.OptIn)]
public abstract class TaskCounterEntity : TaskEntity
{
    [JsonProperty] 
    public int TargetValue  { get; protected set; }
    
    [JsonProperty(PropertyName = "CurrentValue")] 
    private int currentValue;

    [JsonProperty]
    public CompareOperatorType CompareOperator { get; protected set; } = CompareOperatorType.GreaterOrEqual;
    
    public int CurrentValue
    {
        get { return currentValue; }
        set
        {
            if (currentValue == value)
            {
                return;
            }

            if (State == TaskState.New)
            {
                State = TaskState.InProgress;
            }
            
            currentValue = value;
            Check();

            OnChanged?.Invoke(this);
        }
    }
    
#region Serialization

    public bool ShouldSerializeTargetValue()
    {
        return false;
    }
    
    public bool ShouldSerializeCompareOperator()
    {
        return false;
    }
    
#endregion

    protected virtual bool Compare(int current, int target)
    {
        bool result;
        
        switch (CompareOperator)
        {
            case CompareOperatorType.GreaterOrEqual:
                result = current >= target;
                break;
            
            case CompareOperatorType.LessOrEqual:
                result = current <= target;
                break;
            
            case CompareOperatorType.GreaterThan:
                result = current > target;
                break;
            
            case CompareOperatorType.LessThan:
                result = current < target;
                break;
            
            case CompareOperatorType.Equal:
                result = current == target;
                break;
            
            case CompareOperatorType.NotEqual:
                result = current != target;
                break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }

        return result;
    }
    
    protected override bool Check()
    {
        if (IsCompletedOrClaimed())
        {
            return true;
        }

        if (IsInProgress())
        {
            bool result = Compare(currentValue, TargetValue);
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
        string ret = $"{GetType()} [{Id}], State: {State}, Progress: {CurrentValue}/{TargetValue}";
        return ret;
    }
}