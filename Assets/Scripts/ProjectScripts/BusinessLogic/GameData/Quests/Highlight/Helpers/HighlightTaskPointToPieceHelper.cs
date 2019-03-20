using Debug = IW.Logger;
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
    
    /// <summary>
    /// Find A1, A2, A3 when pieceId == A4. If includeTarget is set, A4 will also be included
    /// </summary>
    public static bool FindAndPointToRandomPredecessorPiece(int pieceId, bool includeTarget)
    {
        var boardLogic = BoardService.Current.FirstBoard.BoardLogic;
        var board = BoardService.Current.FirstBoard;
        var matchDef = board.BoardLogic.GetComponent<MatchDefinitionComponent>(MatchDefinitionComponent.ComponentGuid);
        var chain = matchDef.GetChain(pieceId, false);   

        // Get all pieces from the beginning of chain to target piece
        var croppedChain = new List<int>();
        foreach (var id in chain)
        {
            if (pieceId == id)
            {
                if (includeTarget)
                {
                    croppedChain.Add(id);
                }
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
    
    /// <summary>
    /// If we set pieceId = A5 and we have A1, A2, A7 on board, A7 will be highlighted
    /// No random here! First available (or first locked if no avaulable) piece will be used
    /// </summary>
    public static bool FindAndPointToLastPieceInChain(int pieceId)
    {
        var boardLogic = BoardService.Current.FirstBoard.BoardLogic;
        var definition = boardLogic.MatchDefinition;
        var idToCheck = definition.GetLast(pieceId);

        BoardPosition? backup = null; 
        
        while (idToCheck != PieceType.None.Id)
        {
            var piecesPositions = boardLogic.PositionsCache.GetPiecePositionsByType(idToCheck);

            if (piecesPositions.Count > 0)
            {
                var accessiblePoints = HighlightTaskPathHelper.GetAccessiblePositions(piecesPositions);
                if (accessiblePoints.Count > 0)
                {
                    HintArrowView.Show(accessiblePoints[0], 0f, -0.5f);
                    return true;
                }

                if (!backup.HasValue)
                {
                    backup = piecesPositions[0];
                }
            }

            idToCheck = definition.GetPrevious(idToCheck);
        }

        if (backup.HasValue)
        {
            HintArrowView.Show(backup.Value, 0f, -0.5f);
            return true;
        }

        return false;
    }
}