public class BoardConditionComponent: ECSEntity
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    private BoardController context;
    public BoardController Context => context;

    public BoardConditionComponent(BoardController context)
    {
        this.context = context;
    }

    public virtual bool Check(BoardPosition position)
    {
        return CheckMapLimits(position);
    }

    protected bool CheckMapLimits(BoardPosition position)
    {
        return position.IsValidFor(context.BoardDef.Width, context.BoardDef.Height);
    }
}
