using IW;

public class HighlightTaskPointToRandomTree : HighlightTaskUsingPieceFilter
{
    protected override bool ShowArrow(TaskEntity task, float delay)
    {
        includeFilter = PieceTypeFilter.Tree;
        excludeFilter = PieceTypeFilter.ProductionField;
        
        return base.ShowArrow(task, delay);
    }
}