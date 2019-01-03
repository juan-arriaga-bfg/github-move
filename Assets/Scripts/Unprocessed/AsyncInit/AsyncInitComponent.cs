public abstract class AsyncInitComponent : IECSComponent, IHaveProgress
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;
    
    public abstract bool IsCompleted { get; }
    
    public virtual float Progress => IsCompleted ? 1 : 0;

    public virtual void OnRegisterEntity(ECSEntity entity)
    {

    }

    public virtual void OnUnRegisterEntity(ECSEntity entity)
    {

    }
}