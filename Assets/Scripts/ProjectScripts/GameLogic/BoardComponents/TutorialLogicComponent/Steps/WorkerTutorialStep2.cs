using System.Collections.Generic;

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
            Amount = 0,
            MoreThan = true
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

        completeCondition.Amount = Context.Context.BoardLogic.PositionsCache.GetUnlockedPiecePositionsByType(PieceType.Boost_WR.Id).Count - 1;
        
        base.Perform();
    }

    public override void Execute()
    {
        var cache = Context.Context.BoardLogic.PositionsCache;
        var wrs = cache.GetUnlockedPiecePositionsByType(PieceType.Boost_WR.Id);
        var min = float.MaxValue;

        foreach (var wrPosition in wrs)
        {
            var positions = cache.GetUnlockedNearestByFilter(PieceTypeFilter.Fake | PieceTypeFilter.Workplace, wrPosition, 3);
            var nearest = CheckNearestState(positions);

            if (nearest == null) continue;

            var distance = BoardPosition.SqrMagnitude(wrPosition, nearest.Value);

            if (min <= distance) continue;

            min = distance;

            from = wrPosition;
            to = nearest.Value;
        }

        if (min < float.MaxValue)
        {
            base.Execute();
            return;
        }
        
        PauseOff();
    }

    private BoardPosition? CheckNearestState(List<BoardPosition> positions)
    {
        if (positions == null) return null;
        
        foreach (var position in positions)
        {
            var target = Context.Context.BoardLogic.GetPieceAt(position);

            if (target.PieceState == null 
                || target.PieceState.State == BuildingState.InProgress
                || target.PieceState.State == BuildingState.Complete) continue;
            
            return position;
        }
        
        return null;
    }
}