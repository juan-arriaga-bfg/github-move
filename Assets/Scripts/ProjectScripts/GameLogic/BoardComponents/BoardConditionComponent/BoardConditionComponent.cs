public class BoardConditionComponent: ECSEntity
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();

    public override int Guid
    {
        get { return ComponentGuid; }
    }
    
    private BoardController context;

    public BoardController Context
    {
        get { return context; }
    }

    public BoardConditionComponent(BoardController context)
    {
        this.context = context;
    }

    public void OnRegisterEntity(ECSEntity entity)
    {
    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }

    public virtual bool Check(BoardPosition position)
    {
        return CheckMapLimits(position);
    }

    protected bool CheckMapLimits(BoardPosition position)
    {
        var boardWidth = context.BoardDef.Width;
        var boardHeight = context.BoardDef.Height;
        return position.X >= 0 && position.X < boardWidth && position.Y >= 0 && position.Y < boardHeight;
    }
}
