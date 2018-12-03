using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Highlight random chest on the field
/// </summary>
[TaskHighlight(typeof(HighlightTaskPointToShopButton))]
[TaskHighlight(typeof(HighlightTaskFindObstacleForPieceType))]
[TaskHighlight(typeof(HighlightTaskFirstMineOfAnyType))]
public class HighlightTaskOpenChest : HighlightTaskUsingPieceFilter
{
    protected override bool ShowArrow(TaskEntity task, float delay)
    {
        filter = PieceTypeFilter.Chest;
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