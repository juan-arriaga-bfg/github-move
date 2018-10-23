/// <summary>
/// Dummy highlight 
/// </summary>
public class HighlightTaskNotImplemented : ITaskHighlight
{
    public bool Highlight(TaskEntity task)
    {
        UIMessageWindowController.CreateMessage("[Debug]", "Not implemented yet");
        return true;
    }
}