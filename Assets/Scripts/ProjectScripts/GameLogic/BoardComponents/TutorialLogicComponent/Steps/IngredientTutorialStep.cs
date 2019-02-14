using UnityEngine;

public class IngredientTutorialStep : DelayTutorialStep
{
    protected override void OnFirstStart()
    {
        //nothing to do
    }
    
    public override void Execute()
    {
        base.Execute();
        
        var positions = Context.Context.BoardLogic.PositionsCache.GetPiecePositionsByFilter(PieceTypeFilter.Ingredient);

        if (positions.Count > 0 && IsFirstStartEvent())
        {
            var tutorialLogic = BoardService.Current.FirstBoard.TutorialLogic;
            var started = tutorialLogic.SaveStarted;
            started.Add(Id);
            OnFirstStartCallback?.Invoke();
        }
        
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