public class HighlightTaskNextFog : TaskHighlightUsingArrow
{
    protected override bool ShowArrow(TaskEntity task, float delay)
    {
        return HighlightFogHelper.HighlightNextFog(delay);
    }
}