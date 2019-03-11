using System;

public class CrystalTutorialStep : LoopFingerTutorialStep
{
    private readonly int crystal = PieceType.Boost_CR.Id;
    private readonly int target = PieceType.A6.Id;

    private readonly BoardPosition targetPosition = new BoardPosition(20, 10, BoardLayer.Piece.Layer);
    private readonly BoardPosition ignorePosition = new BoardPosition(19, 10, BoardLayer.Piece.Layer);

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
        
        startTime = DateTime.UtcNow.AddMilliseconds(10);

        BoardService.Current?.FirstBoard?.BoardLogic.FireflyLogic.Locker.Lock(this);
        BoardService.Current?.FirstBoard?.BoardLogic.FireflyLogic.DestroyAll();

        UIService.Get.GetCachedModel<UIMainWindowModel>(UIWindowType.MainWindow).IsTutorial = true;
    }

    public override void Execute()
    {
        if (Context.Context.BoardLogic.IsEmpty(targetPosition)) FirstStep();
        else SecondStep();

        base.Execute();
    }

    private void FirstStep()
    {
        var unlock = Context.Context.BoardLogic.PositionsCache.GetPiecePositionsByType(target);
        var positions = Context.Context.BoardLogic.PositionsCache.GetPiecePositionsByType(target);
        
        Context.UnlockAll();
        Context.LockAll();
        
        unlock.Add(ignorePosition);
        unlock.AddRange(Context.Context.BoardLogic.PositionsCache.GetPiecePositionsByType(crystal));

        Context.FadeAll(0.5f, unlock);
        
        positions.Remove(ignorePosition);

        Context.UnlockCell(targetPosition);
        Context.UnlockCells(positions);

        from = positions[0];
        to = targetPosition;
    }

    private void SecondStep()
    {
        var posCrystals = Context.Context.BoardLogic.PositionsCache.GetPiecePositionsByType(crystal);
        var posTargets = Context.Context.BoardLogic.PositionsCache.GetPiecePositionsByType(target);

        Context.UnlockAll();
        Context.LockAll();

        Context.UnlockCells(posCrystals);
        Context.UnlockCells(posTargets);

        from = posCrystals[0];
        to = posTargets[0];
    }

    protected override void Complete()
    {
        Context.FadeAll(1f, null);
        Context.UnlockAll();

        UIService.Get.GetCachedModel<UIMainWindowModel>(UIWindowType.MainWindow).IsTutorial = false;
        BoardService.Current?.FirstBoard?.BoardLogic.FireflyLogic.Locker.Unlock(this);

        base.Complete();
    }
}
