public abstract class ActiveablePieceComponent : IECSComponent, IActiveableCopy
{
    public static int ComponentGuid = ECSManager.GetNextGuid();

    public int Guid
    {
        get { return ComponentGuid; }
    }

    protected Piece context;

    public virtual void OnRegisterEntity(ECSEntity entity)
    {
        this.context = entity as Piece;
    }

    public virtual void OnUnRegisterEntity(ECSEntity entity)
    {
    }

    public abstract void PerformActive(BoardPosition point);
}