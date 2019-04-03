public class HighlightTaskPointToRandomOrderPiece : HighlightTaskUsingPieceFilter
{
    protected override bool ShowArrow(TaskEntity task, float delay)
    {
        includeFilter = PieceTypeFilter.OrderPiece;

        return base.ShowArrow(task, delay);
    }
}