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
    
    public static bool FindAndPointToRandomPredecessorPiece(int pieceId)
    {
        var boardLogic = BoardService.Current.FirstBoard.BoardLogic;
        var board = BoardService.Current.FirstBoard;
        var matchDef = board.BoardLogic.GetComponent<MatchDefinitionComponent>(MatchDefinitionComponent.ComponentGuid);
        var chain = matchDef.GetChain(pieceId);
        
        // Get all pieces from the beginning of chain to target piece
        var croppedChain = new List<int>();
        foreach (var id in chain)
        {
            if (pieceId == id)
            {
                break;
            }
            
            croppedChain.Add(id);
        }

        if (croppedChain.Count == 0)
        {
            Debug.LogError($"[HighlightTaskPointToPieceHelper] => FindAndPointToRandomPredecessorPiece: chain is empty");
            return false;
        }

        List<BoardPosition> inaccessiblePoints = null;
        
        // Loop through chain
        foreach (var id in croppedChain)
        {
            var piecesPositions = boardLogic.PositionsCache.GetPiecePositionsByType(id);
            if (piecesPositions.Count == 0)
            {
                continue;
            }
            
            // If we have accessible pieces, use them as target
            List<BoardPosition> accessiblePoints = HighlightTaskPathHelper.GetAccessiblePositions(piecesPositions);
            if (accessiblePoints.Count > 0)
            {
                ShowArrowForRandomPos(accessiblePoints);
                return true;
            }

            // Store as fallback 
            if (inaccessiblePoints == null)
            {
                inaccessiblePoints = piecesPositions;
            }
        }

        if (inaccessiblePoints != null)
        {
            ShowArrowForRandomPos(inaccessiblePoints);
            return true;
        }

        return false;
    }

    public static void ShowArrowForRandomPos(List<BoardPosition> positions)
    {
        int index = Random.Range(0, positions.Count);
        BoardPosition selectedPosition = positions[index];

        HintArrowView.Show(selectedPosition, 0f, -0.5f);
    }
}