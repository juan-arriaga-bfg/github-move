public class TouchReactionDefinitionComponent : ECSEntity
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();

    public override int Guid { get { return ComponentGuid; } }
    
    public virtual bool Make(BoardPosition position, Piece piece)
    {
        return true;
    }
}