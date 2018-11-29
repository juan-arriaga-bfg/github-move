/// <summary>
/// Dummy highlight 
/// </summary>
public class HighlightTaskNotImplemented : ITaskHighlight
{
    public bool Highlight(TaskEntity task)
    {
        UIMessageWindowController.CreateMessage("Not implemented!", "The hint is not implemented yet.");
        return true;
    }
}