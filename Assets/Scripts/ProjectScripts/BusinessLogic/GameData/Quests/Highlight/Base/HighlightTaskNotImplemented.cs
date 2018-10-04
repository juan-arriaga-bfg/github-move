public class HighlightTaskNotImplemented : ITaskHighlight
{
    public void Highlight(TaskEntity task)
    {
        UIMessageWindowController.CreateMessage("[Debug]", "Not implemented yet");
    }
}