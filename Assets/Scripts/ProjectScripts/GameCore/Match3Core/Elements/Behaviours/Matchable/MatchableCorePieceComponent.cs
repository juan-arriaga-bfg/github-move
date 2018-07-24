public abstract class MatchableCorePieceComponent : IECSComponent, IMatchableCopy
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


	public abstract bool IsMatchable(BoardPosition at);

	public abstract bool OnMatchStart(BoardPosition at);

	public abstract bool OnMatchFinish(BoardPosition at);
}
