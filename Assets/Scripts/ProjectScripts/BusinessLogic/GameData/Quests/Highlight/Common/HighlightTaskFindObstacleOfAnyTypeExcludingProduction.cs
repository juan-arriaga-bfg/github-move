public class HighlightTaskFindObstacleOfAnyTypeExcludingProduction : HighlightTaskUsingPieceFilter
{
    protected override bool ShowArrow(TaskEntity task, float delay)
    {
        includeFilter = PieceTypeFilter.Obstacle;
        excludeFilter = PieceTypeFilter.ProductionField;
        
        return base.ShowArrow(task, delay);
    }
}