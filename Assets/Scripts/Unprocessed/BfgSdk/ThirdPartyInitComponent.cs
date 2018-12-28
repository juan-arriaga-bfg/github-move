public abstract class ThirdPartyInitComponent : IECSComponent
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;
    
    public abstract bool IsCompleted { get; }

    public virtual void OnRegisterEntity(ECSEntity entity)
    {

    }

    public virtual void OnUnRegisterEntity(ECSEntity entity)
    {

    }
}