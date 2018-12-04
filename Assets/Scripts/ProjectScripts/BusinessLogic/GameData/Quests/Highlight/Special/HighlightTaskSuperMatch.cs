using System;

public class HighlightTaskSuperMatch : ITaskHighlight
{
    public bool Highlight(TaskEntity task)
    {
        TaskSuperMatchEntity superMatchTask = task as TaskSuperMatchEntity;
        if (superMatchTask == null)
        {
            return false;
        }

        int count = superMatchTask.CountToMatch;

        string title = string.Format(LocalizationService.Get("highlight.task.supermatch", "highlight.task.supermatch"), count);
        string image = $"codexQuestion";
        
        UIMessageWindowController.CreateImageMessage(title, image, () =>
        {
    
        });

        return true;
    }
}