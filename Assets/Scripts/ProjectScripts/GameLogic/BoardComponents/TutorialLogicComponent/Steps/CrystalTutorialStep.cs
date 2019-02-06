public class CrystalTutorialStep : LoopFingerTutorialStep
{
    private readonly int crystal = PieceType.Boost_CR.Id;
    private readonly int target = PieceType.A5.Id;

    private readonly BoardPosition targetPosition = new BoardPosition(20, 8, BoardLayer.Piece.Layer);
    private readonly BoardPosition ignorePosition = new BoardPosition(19, 8, BoardLayer.Piece.Layer);
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        base.OnRegisterEntity(entity);
        
        RegisterComponent(new CheckPieceTutorialCondition
        {
            ConditionType = TutorialConditionType.Start,
            Target = crystal,
            Amount = 1
        }, true);
        
        RegisterComponent(new CheckPieceTutorialCondition
        {
            ConditionType = TutorialConditionType.Complete,
            Target = crystal,
            Amount = 0
        }, true);
    }
    
    public override void Perform()
    {
        if (IsPerform) return;
        
        base.Perform();
        
        Context.LockAll();
        
        var unlock = Context.Context.BoardLogic.PositionsCache.GetPiecePositionsByType(target);
        
        unlock.Add(ignorePosition);
        unlock.AddRange(Context.Context.BoardLogic.PositionsCache.GetPiecePositionsByType(crystal));
        
        Context.FadeAll(0.5f, unlock);
        
        startTime = startTime.AddSeconds(-(Delay-0.5f));
    }

    public override void Execute()
    {
        if (Context.Context.BoardLogic.IsEmpty(targetPosition)) FirstStep();
        else SecondStep();
        
        base.Execute();
    }
    
    private void FirstStep()
    {
        var positions = Context.Context.BoardLogic.PositionsCache.GetPiecePositionsByType(target);
        
        positions.Remove(ignorePosition);
        
        Context.UnlockCell(targetPosition);
        Context.UnlockCells(positions);
        
        from = positions[0];
        to = targetPosition;
    }

    private void SecondStep()
    {
        var positions = Context.Context.BoardLogic.PositionsCache.GetPiecePositionsByType(crystal);
        
        Context.UnlockAll();
        Context.LockAll();
        
        Context.UnlockCell(targetPosition);
        Context.UnlockCell(ignorePosition);
        Context.UnlockCells(positions);
        
        from = positions[0];
        to = ignorePosition;
    }

    protected override void Complete()
    {
        Context.FadeAll(1f, null);
        Context.UnlockAll();
        
        base.Complete();
    }
}