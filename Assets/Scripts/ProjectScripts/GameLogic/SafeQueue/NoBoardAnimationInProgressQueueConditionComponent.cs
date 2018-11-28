using System;
using System.Collections.Generic;

/// <summary>
/// Ensure that no board animations with specified type are in progress
/// </summary>
public class NoBoardAnimationInProgressQueueConditionComponent : QueueConditionComponent, IECSSystem 
{
    /// <summary>
    /// List of BoardAnimations that block queue
    /// </summary>
    public HashSet<Type> BoardAnimations { get; set; }

    public override bool IsReady()
    {
        var boardRenderer = BoardService.Current?.FirstBoard?.RendererContext;
        if (boardRenderer == null)
        {
            return false;
        }

        var animations = boardRenderer.GetPerformingAnimations();
        for (var i = 0; i < animations.Count; i++)
        {
            var animation = animations[i];
            if (BoardAnimations.Contains(animation.GetType()))
            {
                return false;
            }
        }

        return true;
    }

    public bool IsExecuteable()
    {
        return true;
    }

    public void Execute()
    {
        Check();
    }

    public object GetDependency()
    {
        return null;
    }
}
