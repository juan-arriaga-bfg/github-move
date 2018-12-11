using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Highlight random chest on the field
/// </summary>
public class HighlightTaskOpenChest : HighlightTaskUsingPieceFilter
{
    protected override bool ShowArrow(TaskEntity task, float delay)
    {        
        TaskOpenChestEntity openChestTask = task as TaskOpenChestEntity;
        if (openChestTask == null)
        {
            Debug.LogError($"[HighlightTaskChest] => Unsupported task type: {task.GetType()}");
            return false;
        }

        if (HighlightChestOnField(task, delay))
        {
            return true;
        }

        bool isAnyPiece = openChestTask.PieceId == PieceType.None.Id || openChestTask.PieceId == PieceType.Empty.Id;
        if (isAnyPiece)
        {
            if (new HighlightTaskPointToRandomBranchATree().Highlight(task))
            {
                return true;
            }

            if (new HighlightTaskFirstMineOfAnyType().Highlight(task))
            {
                return true;
            }
        }
        else
        {
            if (new HighlightTaskFindObstacleForPieceType().Highlight(task))
            {
                return true;
            }
            
            if (new HighlightTaskFindMineForPieceType().Highlight(task))
            {
                return true;
            }
        }

        return false;
    }

    private bool HighlightChestOnField(TaskEntity task, float delay)
    {
        TaskOpenChestEntity openChestTask = task as TaskOpenChestEntity;
        includeFilter = PieceTypeFilter.Chest;
        excludeFilter = PieceTypeFilter.Bag;
        
        allowedPieceTypes = openChestTask.PieceId == PieceType.None.Id || openChestTask.PieceId == PieceType.Empty.Id
            ? null
            : new HashSet<int> {openChestTask.PieceId};

        return base.ShowArrow(task, delay);
    }
}