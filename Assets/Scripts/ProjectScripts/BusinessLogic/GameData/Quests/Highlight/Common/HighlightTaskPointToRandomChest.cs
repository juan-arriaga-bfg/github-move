/// <summary>
/// Highlight first existing chest
/// </summary>
public class HighlightTaskPointToRandomChest : HighlightTaskUsingPieceFilter
{
    protected override bool ShowArrow(TaskEntity task, float delay)
    {
        includeFilter = PieceTypeFilter.Chest;
        excludeFilter = PieceTypeFilter.Bag;
        
        return base.ShowArrow(task, delay);
    }
}