/// <summary>
/// Dummy highlight 
/// </summary>
public class HighlightTaskNotImplemented : ITaskHighlight
{
    public bool Highlight(TaskEntity task)
    {
        UIMessageWindowController.CreateNotImplementedMessage();
        return true;
    }
}