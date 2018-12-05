/// <summary>
/// Highlight first existing chest
/// </summary>
public class HighlightTaskPointToRandomChest : HighlightTaskUsingPieceFilter
{
    protected override bool ShowArrow(TaskEntity task, float delay)
    {
        filter = PieceTypeFilter.Chest;
        return base.ShowArrow(task, delay);
    }
}