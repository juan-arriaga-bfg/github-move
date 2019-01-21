using System;
using System.Collections.Generic;

public static class DefaultSafeQueueBuilder
{
    /// <summary>
    /// Schedule action with ID
    /// </summary>
    public static QueueActionComponent BuildAndRun(string id, bool replaceIfExists, Action action)
    {
        var ret = Build(id, replaceIfExists, action);

        ProfileService.Current.QueueComponent.AddAction(ret, replaceIfExists);

        return ret;
    }
    
    /// <summary>
    /// Compose action, but you should run it by yourself!
    /// </summary>
    public static QueueActionComponent Build(string id, bool replaceIfExists, Action action)
    {
        var ret = new QueueActionComponent {Id = id}
                 .AddCondition(new OpenedWindowsQueueConditionComponent
                  {
                      IgnoredWindows = UIWindowType.IgnoredWindows
                  })
                  
                 .AddCondition(new NoBoardAnimationInProgressQueueConditionComponent
                  {
                      BoardAnimations = new HashSet<Type>
                      {
                          typeof(CollapseFogToAnimation),
                      }
                  })
                  
                 .SetAction(action);
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