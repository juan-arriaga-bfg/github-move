using Debug = IW.Logger;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Highlight random obstacle that can produce chest
/// </summary>
public class HighlightTaskPointToRandomBranchATree : TaskHighlightUsingArrow
{
    protected override bool ShowArrow(TaskEntity task, float delay)
    {
        IHavePieceId pieceTask = task as IHavePieceId;
        if (pieceTask == null)
        {
            Debug.LogError("[HighlightTaskFindObstacleForPieceType] => task is not IHavePieceId");
            return false;
        }

        var sourceFilter = PieceTypeFilter.Tree;
        var excludeFilter = PieceTypeFilter.ProductionField;
        var allowedBranches = new List<string>
        {
            "A"
        };
        return HighlightTaskPointToPieceSourceHelper.PointToPieceSource(pieceTask, sourceFilter, excludeFilter, allowedBranches);
    }
}