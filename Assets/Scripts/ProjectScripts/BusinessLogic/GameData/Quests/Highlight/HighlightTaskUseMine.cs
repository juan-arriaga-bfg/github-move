using System.Collections.Generic;
using UnityEngine;

public class HighlightTaskUseMine : TaskHighlightUsingArrow
{
    protected override bool ShowArrow(TaskEntity task, float delay)
    {
        TaskUseMineEntity useMineTask = task as TaskUseMineEntity;
        if (useMineTask == null)
        {
            Debug.LogError("[HighlightTaskClearFog] => task is not TaskUseMineEntity");
            return false;
        }
        
        var board = BoardService.Current.FirstBoard;
        var boardLogic = board.BoardLogic;

        int pieceId = useMineTask.PieceId;
        
        List<BoardPosition> minePositions = pieceId <= 0
            ? boardLogic.PositionsCache.GetPiecePositionsByFilter(PieceTypeFilter.Mine)
            : boardLogic.PositionsCache.GetPiecePositionsByType(pieceId);

        if (minePositions.Count == 0)
        {
            return false;
        }

        // Always highlight if we have only one mine at the field
        if (minePositions.Count == 1)
        {
            HintArrowView.Show(minePositions[0]);
            return true;
        }
        
        // Find accessible mine
        List<BoardPosition> accessibleMinePositions = HighlightTaskPathHelper.GetAccessiblePositions(minePositions);
        if (accessibleMinePositions.Count > 0)
        {
            HintArrowView.Show(accessibleMinePositions[Random.Range(0, accessibleMinePositions.Count)]);
            return true;
        }

        // Path not found, let choose random one
        HintArrowView.Show(minePositions[Random.Range(0, minePositions.Count)]);
        return true;
    }
}