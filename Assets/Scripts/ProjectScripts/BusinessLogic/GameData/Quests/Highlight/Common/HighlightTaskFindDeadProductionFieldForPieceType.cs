using UnityEngine;

public class HighlightTaskFindDeadProductionFieldForPieceType : TaskHighlightUsingArrow
{
    protected override bool ShowArrow(TaskEntity task, float delay)
    {
        IHavePieceId pieceTask = task as IHavePieceId;
        if (pieceTask == null)
        {
            Debug.LogError("[HighlightTaskFindObstacleForPieceType] => task is not IHavePieceId");
            return false;
        }

        PieceTypeDef def = PieceType.GetDefById(pieceTask.PieceId);
        if (!def.Filter.Has(PieceTypeFilter.Ingredient))
        {
            return false;
        }
        
        var sourceFilter = PieceTypeFilter.ProductionField | PieceTypeFilter.Obstacle;
        return HighlightTaskPointToPieceSourceHelper.PointToPieceSource(pieceTask, sourceFilter);
    }
}