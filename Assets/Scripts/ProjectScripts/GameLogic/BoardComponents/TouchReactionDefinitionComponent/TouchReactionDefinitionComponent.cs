public class TouchReactionDefinitionComponent : IECSComponent
{
    public string Icon { get; set; }
    
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;

    public virtual bool IsViewShow(ViewDefinitionComponent viewDefinition)
    {
        return false;
    }
    
    public virtual void OnRegisterEntity(ECSEntity entity)
    {
    }

    public virtual void OnUnRegisterEntity(ECSEntity entity)
    {
    }
    
    public virtual bool Make(BoardPosition position, Piece piece)
    {
        return true;
    }
}