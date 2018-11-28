using System;

public static class DefaultSafeQueueBuilder
{
    /// <summary>
    /// Schedule action with ID
    /// </summary>
    public static QueueActionComponent BuildAndRun(string id, bool replaceIfExists, Action action)
    {
        var ret = new QueueActionComponent {Id = id}
                 .AddCondition(new OpenedWindowsQueueConditionComponent {IgnoredWindows = UIWindowType.IgnoredWindows})
                 .AddCondition(new NoBoardAnimationInProgressQueueConditionComponent {})
                 .SetAction(action);

        ProfileService.Current.QueueComponent.AddAction(ret, replaceIfExists);

        return ret;
    }
    
    /// <summary>
    /// Schedule anonymous action
    /// </summary>
    /// <param name="action"></param>
    public static QueueActionComponent BuildAndRun(Action action)
    {
        return BuildAndRun(null, false, action);
    }
}