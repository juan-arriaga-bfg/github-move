public class HighlightTaskAnyBag : HighlightTaskUsingPieceFilter
{
    protected override bool ShowArrow(TaskEntity task, float delay)
    {
        includeFilter = PieceTypeFilter.Bag;
        
        return base.ShowArrow(task, delay);
    }
}