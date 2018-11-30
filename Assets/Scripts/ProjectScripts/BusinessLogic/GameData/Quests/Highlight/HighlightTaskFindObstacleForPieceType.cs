using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// Highlight random obstacle that can produce Piece specified in the task
/// </summary>
public class HighlightTaskFindObstacleForPieceType : TaskHighlightUsingArrow
{
    protected override bool ShowArrow(TaskEntity task, float delay)
    {
        IHavePieceId pieceTask = task as IHavePieceId;
        if (pieceTask == null)
        {
            Debug.LogError("[HighlightTaskFindObstacleForPieceType] => task is not IHavePieceId");
            return false;
        }

        // Branch of piece
        Regex pieceBranchRegex = new Regex(@"([A-Z]*)?", RegexOptions.IgnoreCase);  
        PieceTypeDef def = PieceType.GetDefById(pieceTask.PieceId);
        string pieceIdStr = def.Abbreviations[0];
        string pieceBranch = pieceBranchRegex.Match(pieceIdStr).Value;
        
        Regex obstacleProductionBranchRegex = new Regex(@"(?<=_)[A-Z].*", RegexOptions.IgnoreCase);
        var board = BoardService.Current.FirstBoard;
        var boardLogic = board.BoardLogic;
        List<BoardPosition> obstaclesPositions = boardLogic.PositionsCache.GetPiecePositionsByFilter(PieceTypeFilter.Obstacle);
        for (int i = obstaclesPositions.Count - 1; i >= 0; i--)
        {
            var obstacle = boardLogic.GetPieceAt(obstaclesPositions[i]);
            var obstaclePieceType = obstacle.PieceType;
            PieceTypeDef obstacleDef = PieceType.GetDefById(obstaclePieceType);
            string obstaclePieceIdStr = obstacleDef.Abbreviations[0];
            string obstacleProductionBranch = obstacleProductionBranchRegex.Match(obstaclePieceIdStr).Value;

            if (pieceBranch != obstacleProductionBranch)
            {
                obstaclesPositions.RemoveAt(i);
            }
        }

        if (obstaclesPositions.Count == 0)
        {
            return false;
        }

        // Always highlight if we have only one obstacle at the field
        if (obstaclesPositions.Count == 1)
        {
            HintArrowView.Show(obstaclesPositions[0]);
            return true;
        }
        
        // Find accessible obstacle
        List<BoardPosition> accessibleObstaclesPositions = HighlightTaskPathHelper.GetAccessiblePositions(obstaclesPositions);
        if (accessibleObstaclesPositions.Count > 0)
        {
            HintArrowView.Show(accessibleObstaclesPositions[Random.Range(0, accessibleObstaclesPositions.Count)]);
            return true;
        }

        // Path not found, let choose random one
        HintArrowView.Show(accessibleObstaclesPositions[Random.Range(0, accessibleObstaclesPositions.Count)]);
        return true;
    }
}