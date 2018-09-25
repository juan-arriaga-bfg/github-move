public abstract class QuestStartConditionComponent : IECSComponent, IECSSerializeable
{    
    public abstract int Guid { get; }
    
    public void OnRegisterEntity(ECSEntity entity)
    {

    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {

    }

    public abstract bool Check();
}