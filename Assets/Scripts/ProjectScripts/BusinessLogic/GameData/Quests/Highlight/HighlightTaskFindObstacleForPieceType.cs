using UnityEngine;

/// <summary>
/// Highlight random obstacle that can produce Piece specified in the task
/// </summary>
public class HighlightTaskFindObstacleForPieceType : TaskHighlightUsingArrow
{
    protected override bool ShowArrow(TaskEntity task, float delay)
    {
        IHavePieceId pieceTask = task as IHavePieceId;
        if (pieceTask == null)
        {
            Debug.LogError("[HighlightTaskFindObstacleForPieceType] => task is not IHavePieceId");
            return false;
        }

        var sourceFilter = PieceTypeFilter.Obstacle;
        return HighlightTaskPointToPieceSourceHelper.PointToPieceSource(pieceTask, sourceFilter);
    }
}