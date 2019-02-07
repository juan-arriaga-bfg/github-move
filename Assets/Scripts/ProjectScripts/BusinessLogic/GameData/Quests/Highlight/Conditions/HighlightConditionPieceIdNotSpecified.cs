using UnityEngine;

public class HighlightConditionPieceIdNotSpecified: ITaskHighlightCondition
{
    public bool Check(TaskEntity task)
    {
        var pieceTask = task as IHavePieceId;
        if (pieceTask == null)
        {
            Debug.LogError($"[HighlightConditionPieceIdNotSpecified] => Task {task.GetType()} is not IHavePieceId");
            return false;
        }

        return pieceTask.PieceId == PieceType.Empty.Id || pieceTask.PieceId == PieceType.None.Id;
    }
}