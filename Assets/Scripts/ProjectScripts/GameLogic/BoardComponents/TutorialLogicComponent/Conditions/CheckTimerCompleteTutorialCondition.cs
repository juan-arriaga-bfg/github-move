using UnityEngine;

public class CheckTimerCompleteTutorialCondition : BaseTutorialCondition
{
    public TimerComponent Target;
    
    public override bool Check()
    {
        Debug.LogError(Target.IsStarted);
        
        return Target == null || Target.Delay > 0 && Target.IsExecuteable() == false;
    }
}