using Debug = IW.Logger;
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
            LocalizationService.Get("quest.supermatch.title", "quest.supermatch.title"),
            UIMessageWindowModel.HintType.SuperMatchHint.ToString());
        
        return true;
    }
}