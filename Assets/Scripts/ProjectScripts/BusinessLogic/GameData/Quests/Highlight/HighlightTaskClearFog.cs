public class HighlightTaskClearFog : ITaskHighlight
{
    public void Highlight(TaskEntity task)
    {
        UIMessageWindowController.CreateMessage("[Debug]", "Clear up some fog!");
    }
}