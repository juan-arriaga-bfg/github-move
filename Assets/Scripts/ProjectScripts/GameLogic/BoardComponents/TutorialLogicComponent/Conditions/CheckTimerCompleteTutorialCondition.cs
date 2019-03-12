public class CheckTimerCompleteTutorialCondition : BaseTutorialCondition
{
    public TimerComponent Target;
    
    public override bool Check()
    {
        return Target == null || Target.Delay > 0 && Target.IsExecuteable() == false;
    }
}