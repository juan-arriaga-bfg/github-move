using System;

/// <summary>
/// Attribute for connection TaskHighlight with a Task. 
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class TaskHighlight : Attribute
{
    private Type highlightType;
    
    private Type[] conditionTypes;

    public Type HighlightType => highlightType;
    
    public Type[] ConditionTypes => conditionTypes;

    public TaskHighlight(Type highlightType)
    {
        Init(highlightType, null);
    }
    
    public TaskHighlight(Type highlightType, Type[] conditions)
    {
        Init(highlightType, conditions);
    }

    private void Init(Type highlightType, Type[] conditions)
    {
        if (highlightType == null)
        {
            throw new ArgumentNullException(nameof(highlightType));
        }

        if (!typeof(ITaskHighlight).IsAssignableFrom(highlightType))
        {
            throw new ArgumentException($"Type {highlightType} not implements ITaskHighlight interface");
        }

        this.highlightType = highlightType;

        if (conditions == null)
        {
            return;
        }

        foreach (var condition in conditions)
        {
           
            if (!typeof(ITaskHighlightCondition).IsAssignableFrom(condition))
            {
                throw new ArgumentException($"Type {condition} not implements ITaskHighlightCondition interface");
            }
        }

        this.conditionTypes = conditions;
    }
}