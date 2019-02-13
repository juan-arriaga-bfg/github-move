public class SelectStorageTutorialStep<T> : BubbleBounceTutorialStep<T>
    where T : UIBoardView
{
    public bool IsFastStart;
    
    private HintArrowView arrow;
    
    public override void PauseOn()
    {
        base.PauseOn();
        
        RemoveArrow();
    }

    public override void Perform()
    {
        if (IsPerform) return;
        
        base.Perform();
        
        if(IsFastStart) startTime = startTime.AddSeconds(-(Delay-0.5f));
    }
    
    public override void Execute()
    {
        base.Execute();
        
        if (bubble != null) return;
        
        foreach (var target in Targets)
        {
            var positions = Context.Context.BoardLogic.PositionsCache.GetRandomPositions(target, 1);
            
            if(positions.Count == 0) continue;
            
            arrow = HintArrowView.Show(positions[0], 0, 0, !Context.Context.Manipulator.CameraManipulator.CameraMove.IsLocked, true);
            break;
        }
    }

    public override bool IsExecuteable()
    {
        return arrow == null && base.IsExecuteable();
    }

    protected override void Complete()
    {
        base.Complete();

        RemoveArrow();
    }

    private void RemoveArrow()
    {
        if (arrow == null) return;
        
        arrow.Remove(0);
        arrow = null;
    }
}