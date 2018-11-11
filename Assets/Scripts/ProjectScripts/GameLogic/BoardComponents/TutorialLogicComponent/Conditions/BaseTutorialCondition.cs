public enum TutorialConditionType
{
    Start,
    Complete,
    Hard
}

public class BaseTutorialCondition : IECSComponent
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;

    public TutorialConditionType ConditionType;
    
    protected BaseTutorialStep context;
    
    public virtual void OnRegisterEntity(ECSEntity entity)
    {
        context = entity as BaseTutorialStep;
    }

    public virtual void OnUnRegisterEntity(ECSEntity entity)
    {
    }
    
    public virtual bool Check()
    {
        return true;
    }
}