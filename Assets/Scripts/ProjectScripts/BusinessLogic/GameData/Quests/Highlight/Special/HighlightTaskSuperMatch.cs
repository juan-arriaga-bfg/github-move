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

        var model = UIService.Get.GetCachedModel<UICodexWindowModel>(UIWindowType.SuperMatchHintWindow);
        UIService.Get.ShowWindow(UIWindowType.SuperMatchHintWindow);

        return true;
    }
}