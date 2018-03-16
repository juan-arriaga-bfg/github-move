public class TouchReactionDefinitionComponent : IECSComponent
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    
    public int Guid { get { return ComponentGuid; } }
    
    public void OnRegisterEntity(ECSEntity entity)
    {
    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }
    
    public virtual bool Make(BoardPosition position, Piece piece)
    {
        return true;
    }
}