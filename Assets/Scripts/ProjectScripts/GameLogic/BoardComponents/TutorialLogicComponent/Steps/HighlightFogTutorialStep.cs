public class HighlightFogTutorialStep : DelayTutorialStep
{
    public override void Perform()
    {
        if (IsPerform) return;
        
        base.Perform();

        Context.Context.HintCooldown.Pause(this);
    }
    
    public override void Execute()
    {
        base.Execute();
        
//        HighlightFogHelper.HighlightNextFog(0);
    }
    
    protected override void Complete()
    {
        base.Complete();
        Context.Context.HintCooldown.Resume(this);
    }
}