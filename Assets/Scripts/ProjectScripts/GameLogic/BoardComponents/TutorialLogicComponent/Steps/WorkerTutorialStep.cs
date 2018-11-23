public class WorkerTutorialStep : LoopFingerTutorialStep
{
    public override void OnRegisterEntity(ECSEntity entity)
    {
        base.OnRegisterEntity(entity);
        
        RegisterComponent(new CheckPieceTutorialCondition
        {
            ConditionType = TutorialConditionType.Start,
            Target = PieceType.Boost_WR.Id,
            Amount = 1
        }, true);
        
        RegisterComponent(new CheckPieceTutorialCondition
        {
            ConditionType = TutorialConditionType.Complete,
            Target = PieceType.Boost_WR.Id,
            Amount = 0
        }, true);
    }
    
    public override void Execute()
    {
        from = Context.Context.BoardLogic.PositionsCache.GetPiecePositionsByType(PieceType.Boost_WR.Id)[0];

        var nearest = Context.Context.BoardLogic.PositionsCache.GetNearestByFilter(PieceTypeFilter.WorkPlace, from);

        if (nearest == null)
        {
            PauseOff();
            return;
        }
        
        to = nearest.Value;
        
        base.Execute();
    }
}