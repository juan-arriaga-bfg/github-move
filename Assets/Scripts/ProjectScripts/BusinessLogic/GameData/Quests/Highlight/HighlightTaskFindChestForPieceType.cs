using UnityEngine;

/// <summary>
/// Highlight random chest that can produce Piece specified in the task
/// </summary>
public class HighlightTaskFindChestForPieceType : TaskHighlightUsingArrow
{
    protected override bool ShowArrow(TaskEntity task, float delay)
    {
        IHavePieceId pieceTask = task as IHavePieceId;
        if (pieceTask == null)
        {
            Debug.LogError("[HighlightTaskFindChestForPieceType] => task is not IHavePieceId");
            return false;
        }

        var sourceFilter = PieceTypeFilter.Chest;
        return HighlightTaskPointToPieceTypeHelper.PointToPieceSource(pieceTask, sourceFilter);
    }
}