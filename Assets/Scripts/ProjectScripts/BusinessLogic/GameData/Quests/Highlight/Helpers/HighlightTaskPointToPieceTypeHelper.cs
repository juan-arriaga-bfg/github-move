using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public static class HighlightTaskPointToPieceTypeHelper
{
    public static bool PointToPieceSource(IHavePieceId pieceTask, PieceTypeFilter sourceFilter)
    {
        // Branch of piece
        Regex pieceBranchRegex = new Regex(@"([A-Z]*)?", RegexOptions.IgnoreCase);
        PieceTypeDef def = PieceType.GetDefById(pieceTask.PieceId);
        string pieceIdStr = def.Abbreviations[0];
        string pieceBranch = pieceBranchRegex.Match(pieceIdStr).Value;

        Regex sourceBranchRegex = new Regex(@"(?<=_)[A-Z].*", RegexOptions.IgnoreCase);
        var board = BoardService.Current.FirstBoard;
        var boardLogic = board.BoardLogic;
        List<BoardPosition> sourcePositions = boardLogic.PositionsCache.GetPiecePositionsByFilter(sourceFilter);
        for (int i = sourcePositions.Count - 1; i >= 0; i--)
        {
            var obstacle = boardLogic.GetPieceAt(sourcePositions[i]);
            var obstaclePieceType = obstacle.PieceType;
            PieceTypeDef obstacleDef = PieceType.GetDefById(obstaclePieceType);
            string obstaclePieceIdStr = obstacleDef.Abbreviations[0];
            string sourceBranch = sourceBranchRegex.Match(obstaclePieceIdStr).Value;

            if (pieceBranch != sourceBranch)
            {
                sourcePositions.RemoveAt(i);
            }
        }

        if (sourcePositions.Count == 0)
        {
            return false;
        }

        // Always highlight if we have only one obstacle at the field
        if (sourcePositions.Count == 1)
        {
            HintArrowView.Show(sourcePositions[0]);
            return true;
        }

        // Find accessible obstacle
        List<BoardPosition> accessiblePositions = HighlightTaskPathHelper.GetAccessiblePositions(sourcePositions);
        if (accessiblePositions.Count > 0)
        {
            HintArrowView.Show(accessiblePositions[Random.Range(0, accessiblePositions.Count)]);
            return true;
        }

        // Path not found, let choose random one
        HintArrowView.Show(accessiblePositions[Random.Range(0, accessiblePositions.Count)]);
        return true;
    }
}