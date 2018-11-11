using System;

public class DelayTutorialStep : BaseTutorialStep, IECSSystem
{
    public int Delay;
    private DateTime startTime;
    
    public override void PauseOff()
    {
        base.PauseOff();
        
        startTime = DateTime.UtcNow;
    }
    
    public override void Perform()
    {
        if (IsPerform) return;
        
        base.Perform();
        
        startTime = DateTime.UtcNow;
    }
    
    public virtual bool IsExecuteable()
    {
        return IsPerform && (DateTime.UtcNow - startTime).TotalSeconds >= Delay;
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