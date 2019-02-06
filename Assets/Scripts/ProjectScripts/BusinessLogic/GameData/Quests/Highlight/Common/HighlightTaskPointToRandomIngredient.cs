public class HighlightTaskPointToRandomIngredient : HighlightTaskUsingPieceFilter
{
    protected override bool ShowArrow(TaskEntity task, float delay)
    {
        includeFilter = PieceTypeFilter.Ingredient;
        excludeFilter = PieceTypeFilter.ProductionField;
        
        return base.ShowArrow(task, delay);
    }
}