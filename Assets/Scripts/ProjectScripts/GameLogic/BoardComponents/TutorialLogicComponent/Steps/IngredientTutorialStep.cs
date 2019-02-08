public class IngredientTutorialStep : DelayTutorialStep
{   
    public override void Execute()
    {
        base.Execute();

        var positions = Context.Context.BoardLogic.PositionsCache.GetPiecePositionsByFilter(PieceTypeFilter.Ingredient);
            
        foreach (var position in positions)
        {
            var element = Context.Context.BoardLogic.GetPieceAt(position).ActorView;
                        
            element.AddArrow();
        }
    }
    
    protected override void Complete()
    {
        base.Complete();
        
        var positions = Context.Context.BoardLogic.PositionsCache.GetPiecePositionsByFilter(PieceTypeFilter.Ingredient);

        foreach (var position in positions)
        {
            var element = Context.Context.BoardLogic.GetPieceAt(position).ActorView;
                
            element.RemoveArrow();
        }
    }
}