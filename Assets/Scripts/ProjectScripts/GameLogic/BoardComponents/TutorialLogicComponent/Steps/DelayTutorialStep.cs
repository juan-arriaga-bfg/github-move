using System;

public class DelayTutorialStep : BaseTutorialStep, IECSSystem
{
    public int Delay;
    protected DateTime startTime;

    private bool isPaused;

    public override void PauseOn()
    {
        isPaused = true;
    }

    public override void PauseOff()
    {
        base.PauseOff();
        
        startTime = DateTime.UtcNow;
        isPaused = false;
    }
    
    public override void Perform()
    {
        if (IsPerform) return;
        
        base.Perform();
        
        startTime = DateTime.UtcNow;
    }
    
    public virtual bool IsExecuteable()
    {
        return isPaused == false && IsPerform && (DateTime.UtcNow - startTime).TotalSeconds >= Delay;
    }
    
    public virtual void Execute()
    {
        PauseOff();
    }
    
    public object GetDependency()
    {
        return null;
    }
}