using UnityEngine;

public class HighlightConditionPieceFromProductBranch: ITaskHighlightCondition
{
    public bool Check(TaskEntity task)
    {
        var pieceTask = task as IHavePieceId;
        if (pieceTask == null)
        {
            Debug.LogError($"[HighlightConditionPieceFromProductBranch] => Task {task.GetType()} is not IHavePieceId");
            return false;
        }

        var pieceStr = PieceType.Parse(pieceTask.PieceId);
        return pieceStr.StartsWith("PR_");
    }
}