using UnityEngine;

public class HighlightTaskSuperMatch : ITaskHighlight
{
    public bool Highlight(TaskEntity task)
    {
        TaskSuperMatchEntity superMatchTask = task as TaskSuperMatchEntity;
        if (superMatchTask == null)
        {
            Debug.LogError($"[HighlightTaskSuperMatch] => Unsupported task type: {task.GetType()}");
            return false;
        }

        UIMessageWindowController.CreatePrefabMessage(
            LocalizationService.Get("highlight.task.supermatch.window.title", "highlight.task.supermatch.window.title"),
            UIMessageWindowModel.HintType.SuperMatchHint.ToString());
        
        return true;
    }
}