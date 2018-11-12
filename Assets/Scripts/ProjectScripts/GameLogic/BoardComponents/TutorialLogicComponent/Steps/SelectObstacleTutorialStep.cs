using System.Collections.Generic;

public class SelectObstacleTutorialStep : DelayTutorialStep
{
    public List<int> Targets;

    private HintArrowView arrow;
    
    public override void Perform()
    {
        if (IsPerform) return;
        
        base.Perform();

        Context.Context.HintCooldown.Pause(this);
    }
    
    public override void Execute()
    {
        base.Execute();
        
        foreach (var target in Targets)
        {
            var positions = Context.Context.BoardLogic.PositionsCache.GetRandomPositions(target, 1);
            
            if(positions.Count == 0) continue;

            arrow = HintArrowView.Show(positions[0], 0, 0, true, true);
            break;
        }
    }
    
    protected override void Complete()
    {
        base.Complete();
        
        if (arrow != null)
        {
            arrow.Remove(0);
            arrow = null;
        }
        
        Context.Context.HintCooldown.Resume(this);
    }
}