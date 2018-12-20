using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public static class HighlightTaskPointToPieceSourceHelper
{
    // Match: A1, B1, C9, returns: A, B, C
    private static readonly Regex pieceBranchRegexSimple  = new Regex(@"(^)[A-Z]*",      RegexOptions.IgnoreCase);
    
    // Match: PR_A1, CH2_B, returns: A, B
    private static readonly Regex pieceBranchRegexComplex = new Regex(@"(?<=_)([A-Z])*", RegexOptions.IgnoreCase);

    public static bool PointToPieceSource(IHavePieceId pieceTask, PieceTypeFilter sourceFilter, PieceTypeFilter? excludeFilter = null, List<string> allowedSourceBranches = null)
    {        
        int pieceId = pieceTask.PieceId;
        return PointToPieceSource(pieceId, sourceFilter, excludeFilter, allowedSourceBranches);
    }

    public static bool PointToPieceSource(int pieceId, PieceTypeFilter sourceFilter, PieceTypeFilter? excludeFilter = null, List<string> allowedSourceBranches = null)
    {       
        // Branch of piece
        PieceTypeDef def = PieceType.GetDefById(pieceId);
        string pieceIdStr = def.Abbreviations[0];

        Regex pieceRegex = pieceIdStr.Contains("_") ? pieceBranchRegexComplex : pieceBranchRegexSimple;
        string pieceBranch = pieceRegex.Match(pieceIdStr).Value;

        Regex sourceBranchRegex = pieceBranchRegexComplex;
        
        var board = BoardService.Current.FirstBoard;
        var boardLogic = board.BoardLogic;
        
        List<BoardPosition> sourcePositions = boardLogic.PositionsCache.GetPiecePositionsByFilter(sourceFilter);
        
        for (int i = sourcePositions.Count - 1; i >= 0; i--)
        {
            var source = boardLogic.GetPieceAt(sourcePositions[i]);
            var sourcePieceType = source.PieceType;
            PieceTypeDef sourceDef = PieceType.GetDefById(sourcePieceType);
            
            if (excludeFilter != null)
            {
                if (sourceDef.Filter.Has(excludeFilter))
                {
                    sourcePositions.RemoveAt(i);
                    continue;
                }
            }
            
            string sourcePieceIdStr = sourceDef.Abbreviations[0];
            string sourceBranch = sourceBranchRegex.Match(sourcePieceIdStr).Value;

            // Hack for tutorials
            if (sourceBranch == "TT")
            {
                sourceBranch = "A";
            }
            
            if (allowedSourceBranches != null && !allowedSourceBranches.Contains(sourceBranch))
            {
                sourcePositions.RemoveAt(i);
                continue;
            }

            if (pieceId == PieceType.Empty.Id || pieceId == PieceType.None.Id)
            {
                continue;
            }

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