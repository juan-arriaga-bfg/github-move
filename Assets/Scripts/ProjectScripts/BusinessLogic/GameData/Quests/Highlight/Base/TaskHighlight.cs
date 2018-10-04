using System;

/// <summary>
/// Attribute for connection TaskHighlight with a Task. 
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class TaskHighlight : Attribute
{
    private readonly Type highlightType;

    public Type HighlightType => highlightType;

    public TaskHighlight(Type highlightType)
    {
        if (highlightType == null)
        {
            throw new ArgumentNullException(nameof (highlightType));
        }

        if (!typeof(ITaskHighlight).IsAssignableFrom(highlightType))
        {
            throw new ArgumentException($"Type {highlightType} not implements ITaskHighlight interface");
        }
        
        this.highlightType = highlightType;
    }
}