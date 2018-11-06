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
        
        string title = $"Match {count}+ pieces at once";
        string image = $"codexQuestion";
        
        UIMessageWindowController.CreateImageMessage(title, image, () =>
        {
    
        });

        return true;
    }
}