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

        var model = UIService.Get.GetCachedModel<UICodexWindowModel>(UIWindowType.FireflyHintWindow);
        UIService.Get.ShowWindow(UIWindowType.FireflyHintWindow);

        return true;
    }
}