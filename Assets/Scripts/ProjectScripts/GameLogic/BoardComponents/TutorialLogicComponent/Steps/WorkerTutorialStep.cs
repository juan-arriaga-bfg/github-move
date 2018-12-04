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

        var nearest = Context.Context.BoardLogic.PositionsCache.GetNearestByFilter(PieceTypeFilter.WorkPlace, from, 50);

        if (nearest == null)
        {
            PauseOff();
            return;
        }
        
        foreach (var position in nearest)
        {
            var target = Context.Context.BoardLogic.GetPieceAt(position);

            if (!target.Context.PathfindLocker.HasPath(target)) continue;
        
            var life = target.GetComponent<StorageLifeComponent>(StorageLifeComponent.ComponentGuid);

            if (life != null && !life.Locker.IsLocked && (!life.IsUseCooldown || !life.Timer.IsExecuteable()))
            {
                to = position;
                base.Execute();
                return;
            }
        
            var state = target.GetComponent<PieceStateComponent>(PieceStateComponent.ComponentGuid);
        
            if (state != null && state.State != BuildingState.InProgress && state.State != BuildingState.Complete)
            {
                to = position;
                base.Execute();
                return;
            }
        }
    }
}