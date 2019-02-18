public class CheckStepTutorialCondition : BaseTutorialCondition
{
    public int Target;
    
    public override bool Check()
    {
        return context.Context.SaveCompleted.Contains(Target);
    }
}