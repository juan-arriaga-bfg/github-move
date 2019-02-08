public class IngredientTutorialStep : BaseTutorialStep
{
    public override void Perform()
    {
        if (IsPerform == false) base.Perform();
        if (IsPerform == false) return;
        
        Context.Context.ActionExecutor.AddAction(new CallbackAction
        {
            Delay = 0.2f,
            Callback = controller =>
            {
                var positions = Context.Context.BoardLogic.PositionsCache.GetPiecePositionsByFilter(PieceTypeFilter.Ingredient);

                foreach (var position in positions)
                {
                    var element = Context.Context.BoardLogic.GetPieceAt(position).ActorView;
                        
                    element.AddArrow();
                }
            }
        });
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