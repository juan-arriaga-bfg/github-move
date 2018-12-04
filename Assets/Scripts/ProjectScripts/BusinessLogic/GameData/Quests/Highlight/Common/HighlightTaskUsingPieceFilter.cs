using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

/// <summary>
/// Return random piece with filter. Accessible pieces have top priority
/// </summary>
public abstract class HighlightTaskUsingPieceFilter : TaskHighlightUsingArrow
{
    protected PieceTypeFilter filter = PieceTypeFilter.Default;
    
    protected HashSet<int> allowedPieceTypes = null;

    protected override bool ShowArrow(TaskEntity task, float delay)
    {
        var boardLogic = BoardService.Current.FirstBoard.BoardLogic;
        var positionsByFilter = boardLogic.PositionsCache.GetPiecePositionsByFilter(filter);

        if (allowedPieceTypes != null)
        {
            for (int i = positionsByFilter.Count - 1; i >= 0; i--)
            {
                var piece = boardLogic.GetPieceAt(positionsByFilter[i]);
                var type = piece.PieceType;

                if (!allowedPieceTypes.Contains(type))
                {
                    positionsByFilter.RemoveAt(i);
                }
            }
        }
        
        if (positionsByFilter.Count == 0)
        {
            return false;
        }

        // If we have accessible positions, use them as target
        var  accessiblePositions = HighlightTaskPathHelper.GetAccessiblePositions(positionsByFilter);
        List<BoardPosition> positions = accessiblePositions.Count > 0 ? accessiblePositions : positionsByFilter;

        int  index = Random.Range(0, positions.Count);
        BoardPosition selectedPosition = positions[index];

        HintArrowView.Show(selectedPosition);
        
        return true;
    }
}