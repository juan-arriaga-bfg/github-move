public enum TutorialConditionType
{
    Start,
    Complete
}

public class BaseTutorialCondition : IECSComponent
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;

    public TutorialConditionType ConditionType;
    
    protected BaseTutorialStep context;
    
    public void OnRegisterEntity(ECSEntity entity)
    {
        context = entity as BaseTutorialStep;
    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }
    
    public virtual bool Check()
    {
        return true;
    }
}