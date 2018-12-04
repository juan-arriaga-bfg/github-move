using System.Collections.Generic;
using UnityEngine;

public static class HighlightTaskPointToPieceHelper
{
    public static bool FindAndPointToRandomPiece(int pieceId)
    {
        var boardLogic = BoardService.Current.FirstBoard.BoardLogic;
        var piecesPositions = boardLogic.PositionsCache.GetPiecePositionsByType(pieceId);

        if (piecesPositions.Count == 0)
        {
            return false;
        }

        // If we have accessible pieces, use them as target
        var accessiblePoints = HighlightTaskPathHelper.GetAccessiblePositions(piecesPositions);
        List<BoardPosition> positions = accessiblePoints.Count > 0 ? accessiblePoints : piecesPositions;

        int index = Random.Range(0, positions.Count);
        BoardPosition selectedPosition = positions[index];

        HintArrowView.Show(selectedPosition, 0f, -0.5f);

        return true;
    }
}