using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ensure that no board animations with specified type are in progress
/// </summary>
public class NoBoardAnimationInProgressQueueConditionComponent : QueueConditionComponent, IECSSystem 
{
    /// <summary>
    /// List of BoardAnimations that block queue
    /// </summary>
    public HashSet<Type> BoardAnimations { get; set; }

    private BoardRenderer cachedRenderer;

    public BoardRenderer CachedRenderer => cachedRenderer ?? (cachedRenderer = BoardService.Current?.FirstBoard?.RendererContext);

    public override bool IsReady()
    {
        if (CachedRenderer == null)
        {
            // Debug.Log($"[NoBoardAnimationInProgressQueueConditionComponent] => IsReady == false");
            return false;
        }

        var animations = CachedRenderer.GetPerformingAnimations();
        for (var i = 0; i < animations.Count; i++)
        {
            var animation = animations[i];
            if (BoardAnimations.Contains(animation.GetType()))
            {
                // Debug.Log($"[NoBoardAnimationInProgressQueueConditionComponent] => IsReady == false");
                return false;
            }
        }

        // Debug.Log($"[NoBoardAnimationInProgressQueueConditionComponent] => IsReady == true");
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
