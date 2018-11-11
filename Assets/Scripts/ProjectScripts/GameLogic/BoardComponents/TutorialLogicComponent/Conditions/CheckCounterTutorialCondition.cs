public class CheckCounterTutorialCondition : BaseTutorialCondition
{
    public int Target;
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        base.OnRegisterEntity(entity);

        context.Repeat = Target;
    }

    public override bool Check()
    {
        return context.Repeat == 0;
    }
}