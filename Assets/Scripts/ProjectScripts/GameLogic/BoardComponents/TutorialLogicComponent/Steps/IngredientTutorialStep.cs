using System.Collections.Generic;

public class IngredientTutorialStep : BaseTutorialStep
{
    public override void OnRegisterEntity(ECSEntity entity)
    {
        base.OnRegisterEntity(entity);
        
        RegisterComponent(
            new CheckCurrencyTutorialCondition
            {
                Target = 3,
                Currency = new List<string>
                {
                    Currency.PR_A5.Name,
                    Currency.PR_B5.Name,
                    Currency.PR_C5.Name,
                    Currency.PR_D5.Name,
                    Currency.PR_E5.Name,
                },
                ConditionType = TutorialConditionType.Complete
            }, true);
    }

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