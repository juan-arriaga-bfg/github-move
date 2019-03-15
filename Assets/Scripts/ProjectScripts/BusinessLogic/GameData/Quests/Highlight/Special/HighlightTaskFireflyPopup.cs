using Debug = IW.Logger;
using UnityEngine;

public class HighlightTaskFireflyPopup : ITaskHighlight
{
    public bool Highlight(TaskEntity task)
    {
        TaskCollectFireflyEntity ffTask = task as TaskCollectFireflyEntity;
        if (ffTask == null)
        {
            Debug.LogError($"[HighlightTaskFireflyPopup] => Unsupported task type: {task.GetType()}");
            return false;
        }
        
        UIMessageWindowController.CreatePrefabMessage(
            LocalizationService.Get("highlight.task.firefly.window.title", "highlight.task.firefly.window.title"),
            UIMessageWindowModel.HintType.FireflyHint.ToString());

        return true;
    }
}