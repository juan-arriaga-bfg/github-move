using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

/// <summary>
/// Return random piece with filter. Accessible pieces have top priority
/// </summary>
public abstract class HighlightTaskUsingPieceFilter : TaskHighlightUsingArrow
{
    protected PieceTypeFilter includeFilter = PieceTypeFilter.Default;
    protected PieceTypeFilter? excludeFilter = null;
    
    protected HashSet<int> allowedPieceTypes = null;

    // Override to get additional conditions for pieces filtering
    protected virtual bool Validate(Piece piece)
    {
        return true;
    }
    
    protected override bool ShowArrow(TaskEntity task, float delay)
    {
        var boardLogic = BoardService.Current.FirstBoard.BoardLogic;
        
        var positionsByFilter = excludeFilter != null 
            ? boardLogic.PositionsCache.GetPiecePositionsByFilter(includeFilter, excludeFilter.Value) 
            : boardLogic.PositionsCache.GetPiecePositionsByFilter(includeFilter);

        for (int i = positionsByFilter.Count - 1; i >= 0; i--)
        {
            var piece = boardLogic.GetPieceAt(positionsByFilter[i]);
            var type = piece.PieceType;
            
            if (!Validate(piece) 
                || (allowedPieceTypes != null && !allowedPieceTypes.Contains(type))
            )
            {
                positionsByFilter.RemoveAt(i);
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