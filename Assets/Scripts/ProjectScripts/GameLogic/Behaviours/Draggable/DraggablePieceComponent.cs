public class DraggablePieceComponent : IECSComponent, IDraggable
{
    public static int ComponentGuid = ECSManager.GetNextGuid();

    public int Guid
    {
        get { return ComponentGuid; }
    }
    
    public virtual void OnRegisterEntity(ECSEntity entity)
    {
    }

    public virtual void OnUnRegisterEntity(ECSEntity entity)
    {
    }
    
    public bool IsDraggable(BoardPosition at)
    {
        return true;
    }
}