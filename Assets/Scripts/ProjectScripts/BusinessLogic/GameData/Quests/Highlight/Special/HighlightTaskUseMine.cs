using Debug = IW.Logger;
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

        if (useMineTask.PieceId == PieceType.None.Id || useMineTask.PieceId == PieceType.Empty.Id)
        {
            return HighlightTaskMineHelper.Highlight(useMineTask.PieceId); 
        }

        var chain = useMineTask.Chain;
        foreach (var id in chain)
        {
            if (HighlightTaskMineHelper.Highlight(id))
            {
                return true;
            }
        }

        return false;
    }
}