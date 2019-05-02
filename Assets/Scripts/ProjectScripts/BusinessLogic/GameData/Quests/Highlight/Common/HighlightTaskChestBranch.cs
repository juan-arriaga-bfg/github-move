using System.Collections.Generic;
using UnityEngine;

public class HighlightTaskChestBranch : TaskHighlightUsingArrow
{    
    protected override bool ShowArrow(TaskEntity task, float delay)
    {
        IHavePieceId pieceTask = task as IHavePieceId;
        if (pieceTask == null)
        {
            Debug.LogError("[HighlightTaskChestBranch] => task is not IHavePieceId");
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
            Debug.LogError($"[HighlightTaskChestBranch] => ShowArrow: Branch for {uid} is empty!");
            return false;
        }

        var sourceFilter = PieceTypeFilter.Chest;
        var excludeFilter = PieceTypeFilter.Bag;
        var allowedBranches = new List<string>
        {
            targetBranch
        };
        return HighlightTaskPointToPieceSourceHelper.PointToPieceSource(pieceTask, sourceFilter, excludeFilter, allowedBranches);
    }
}