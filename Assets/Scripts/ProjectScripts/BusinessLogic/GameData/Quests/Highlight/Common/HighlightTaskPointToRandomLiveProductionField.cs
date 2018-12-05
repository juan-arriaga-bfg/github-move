/// <summary>
/// Highlight first existing live ProductionField piece 
/// </summary>
public class HighlightTaskPointToRandomLiveProductionField : HighlightTaskUsingPieceFilter
{
    protected override bool Validate(Piece piece)
    {
        var def = PieceType.GetDefById(piece.PieceType);
        if (def.Filter.Has(PieceTypeFilter.Obstacle))
        {
            return false;
        }

        return true;
    }

    protected override bool ShowArrow(TaskEntity task, float delay)
    {
        filter = PieceTypeFilter.ProductionField;
        return base.ShowArrow(task, delay);
    }
}