public class HighlightTaskPointToRandomDeadProductionField : HighlightTaskUsingPieceFilter
{
    protected override bool ShowArrow(TaskEntity task, float delay)
    {
        includeFilter = PieceTypeFilter.ProductionField | PieceTypeFilter.Obstacle;
        return base.ShowArrow(task, delay);
    }
}