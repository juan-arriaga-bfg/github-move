public class MatchablePieceComponent : IECSComponent
{
	public static int ComponentGuid = ECSManager.GetNextGuid();

	public int Guid
	{
		get { return ComponentGuid; }
	}

	protected Piece Context;
	private bool isLast;
    
	public virtual void OnRegisterEntity(ECSEntity entity)
	{
		Context = entity as Piece;
		isLast = Context.Context.BoardLogic.MatchDefinition.GetNext(Context.PieceType, false) == PieceType.None.Id;
	}

	public virtual void OnUnRegisterEntity(ECSEntity entity)
	{
	}
    
	public virtual bool IsMatchable()
	{
		return !isLast;
	}
}