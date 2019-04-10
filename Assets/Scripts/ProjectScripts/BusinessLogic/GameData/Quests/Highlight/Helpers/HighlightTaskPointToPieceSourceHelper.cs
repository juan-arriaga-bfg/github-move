using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public static class HighlightTaskPointToPieceSourceHelper
{
    // Match: A1, B1, C9, returns: A, B, C
    public static readonly Regex PieceBranchRegexSimple  = new Regex(@"(^)[A-Z]*", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    
    // Match: PR_A1, CH2_B, OB_PR_C, returns: A, B, C
    public static readonly Regex PieceBranchRegexComplex = new Regex(@"(?<=_)([A-Z]*)($|(?=[0-9]+))", RegexOptions.IgnoreCase | RegexOptions.Compiled);

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

        Regex pieceRegex = pieceIdStr.Contains("_") ? PieceBranchRegexComplex : PieceBranchRegexSimple;
        string pieceBranch = pieceRegex.Match(pieceIdStr).Value;

        Regex sourceBranchRegex = PieceBranchRegexComplex;
        
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

            // // Hack for tutorials
            // if (sourceBranch == "TT")
            // {
            //     sourceBranch = "A";
            // }
            
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