public class WorkerTutorialStep2 : LoopFingerTutorialStep
{
    private CheckPieceTutorialCondition completeCondition;
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        base.OnRegisterEntity(entity);
        
        RegisterComponent(new CheckPieceTutorialCondition
        {
            ConditionType = TutorialConditionType.Start,
            Target = PieceType.Boost_WR.Id,
            Amount = 1
        }, true);

        completeCondition = new CheckPieceTutorialCondition
        {
            ConditionType = TutorialConditionType.Complete,
            Target = PieceType.Boost_WR.Id,
            Amount = 0
        };
        
        RegisterComponent(completeCondition, true);
    }

    public override void Perform()
    {
        if (IsPerform) return;

        completeCondition.Amount = Context.Context.BoardLogic.PositionsCache.GetPiecePositionsByType(PieceType.Boost_WR.Id).Count - 1;
        
        base.Perform();
    }

    public override void Execute()
    {
        from = Context.Context.BoardLogic.PositionsCache.GetPiecePositionsByType(PieceType.Boost_WR.Id)[0];

        var nearest = Context.Context.BoardLogic.PositionsCache.GetNearestByFilter(PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace, from, 50);

        if (nearest == null)
        {
            PauseOff();
            return;
        }
        
        foreach (var position in nearest)
        {
            var target = Context.Context.BoardLogic.GetPieceAt(position);

            if (target.Context.PathfindLocker.HasPath(target) == false
                || target.PieceState == null 
                || target.PieceState.State == BuildingState.InProgress
                || target.PieceState.State == BuildingState.Complete) continue;
            
            to = position;
            base.Execute();
            return;
        }
    }
}