using System.Collections.Generic;
using UnityEngine;

public class HighlightTaskCutTree : TaskHighlightUsingArrow
{   
    protected override bool ShowArrow(TaskEntity task, float delay)
    {
        var boardLogic = BoardService.Current.FirstBoard.BoardLogic;
        var workPlacesList = boardLogic.PositionsCache.GetPiecePositionsByFilter(PieceTypeFilter.Tree);
        
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