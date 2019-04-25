using Debug = IW.Logger;
using UnityEngine;

public class HighlightTaskFindDeadProductionFieldForPieceType : TaskHighlightUsingArrow
{
    protected override bool ShowArrow(TaskEntity task, float delay)
    {
        IHavePieceId pieceTask = task as IHavePieceId;
        if (pieceTask == null)
        {
            Debug.LogError("[HighlightTaskFindDeadProductionFieldForPieceType] => task is not IHavePieceId");
            return false;
        }

        var sourceFilter = PieceTypeFilter.ProductionField | PieceTypeFilter.Obstacle;
        return HighlightTaskPointToPieceSourceHelper.PointToPieceSource(pieceTask, sourceFilter);
    }
}