using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Highlight random chest on the field
/// </summary>
public class HighlightTaskOpenChest : HighlightTaskUsingPieceFilter
{
    protected override bool ShowArrow(TaskEntity task, float delay)
    {
        includeFilter = PieceTypeFilter.Chest;
        excludeFilter = PieceTypeFilter.Bag;
        
        TaskOpenChestEntity openChestTask = task as TaskOpenChestEntity;
        if (openChestTask == null)
        {
            Debug.LogError($"[HighlightTaskChest] => Unsupported task type: {task.GetType()}");
            return false;
        }

        allowedPieceTypes = openChestTask.PieceId == PieceType.None.Id || openChestTask.PieceId == PieceType.Empty.Id 
            ? null 
            : new HashSet<int> {openChestTask.PieceId};

        return base.ShowArrow(task, delay);
    }
}