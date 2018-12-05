public class HighlightTaskPointToRandomDeadProductionField : HighlightTaskUsingPieceFilter
{
    protected override bool ShowArrow(TaskEntity task, float delay)
    {
        filter = PieceTypeFilter.ProductionField | PieceTypeFilter.Obstacle;
        return base.ShowArrow(task, delay);
    }
}