using System.Collections.Generic;
using UnityEngine;

public class HighlightTaskUsingObstaclePieceFilter : TaskHighlightUsingArrow
{
    protected override bool ShowArrow(TaskEntity task, float delay)
    {
        PieceTypeFilter filter;

        if (task is TaskKillTreeEntity || task is TaskHitTreeEntity)
        {
            filter = PieceTypeFilter.Tree;
        }
        else if (task is TaskKillFieldEntity)
        {
            filter = PieceTypeFilter.ProductionField;
        }
        else
        {
            Debug.LogError($"[HighlightTaskUsingPieceFilter] => Unsupported task type: {task.GetType()}");
            return false;
        }
        
        var boardLogic = BoardService.Current.FirstBoard.BoardLogic;
        var workPlacesList = boardLogic.PositionsCache.GetPiecePositionsByFilter(filter);
        
        if (workPlacesList.Count == 0)
        {
            return false;
        }

        // If we have accessible pieces, use them as target
        var  accessiblePoints = HighlightTaskPathHelper.GetAccessiblePositions(workPlacesList);
        List<BoardPosition> positions = accessiblePoints.Count > 0 ? accessiblePoints : workPlacesList;

        int  index = Random.Range(0, positions.Count);
        BoardPosition selectedPosition = positions[index];

        HintArrowView.Show(selectedPosition);
        
        return true;
    }
}