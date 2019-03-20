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

        return HighlightTaskMineHelper.Highlight(useMineTask.PieceId);
    }
}