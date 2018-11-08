public class ChechStepTutorialCondition : BaseTutorialCondition
{
    public int Target;
    
    public override bool Check()
    {
        return context.Context.Save.Contains(Target);
    }
}