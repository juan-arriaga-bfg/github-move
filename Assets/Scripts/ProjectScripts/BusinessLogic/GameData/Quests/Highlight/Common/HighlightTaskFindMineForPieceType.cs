using UnityEngine;

/// <summary>
/// Highlight random mine that can produce Piece specified in the task
/// </summary>
public class HighlightTaskFindMineForPieceType : TaskHighlightUsingArrow
{
    protected override bool ShowArrow(TaskEntity task, float delay)
    {
        IHavePieceId pieceTask = task as IHavePieceId;
        if (pieceTask == null)
        {
            Debug.LogError("[HighlightTaskFindObstacleForPieceType] => task is not IHavePieceId");
            return false;
        }

        var sourceFilter = PieceTypeFilter.Mine;
        return HighlightTaskPointToPieceSourceHelper.PointToPieceSource(pieceTask, sourceFilter);
    }
}