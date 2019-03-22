using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  Highlight tree with specific branch
/// </summary>
public class HighlightTaskTreeBranch : TaskHighlightUsingArrow
{    
    protected override bool ShowArrow(TaskEntity task, float delay)
    {
        IHavePieceId pieceTask = task as IHavePieceId;
        if (pieceTask == null)
        {
            Debug.LogError("[HighlightTaskTreeBranch] => task is not IHavePieceId");
            return false;
        }

        int pieceId = pieceTask.PieceId;

        if (pieceId == PieceType.None.Id || pieceId == PieceType.Empty.Id)
        {
            return false;
        }

        string uid = PieceType.Parse(pieceId);
        var targetBranch = HighlightTaskPointToPieceSourceHelper.PieceBranchRegexComplex.Match(uid).Value;
        if (string.IsNullOrEmpty(targetBranch))
        {
            Debug.LogError($"[HighlightTaskTreeBranch] => ShowArrow: Branch for {uid} is empty!");
            return false;
        }
        
        // fallback for tutorial pieces
        if (targetBranch == "TT")
        {
            targetBranch = "A";
        }

        var sourceFilter = PieceTypeFilter.Tree;
        var excludeFilter = PieceTypeFilter.ProductionField;
        var allowedBranches = new List<string>
        {
            targetBranch
        };
        return HighlightTaskPointToPieceSourceHelper.PointToPieceSource(pieceTask, sourceFilter, excludeFilter, allowedBranches);
    }
}