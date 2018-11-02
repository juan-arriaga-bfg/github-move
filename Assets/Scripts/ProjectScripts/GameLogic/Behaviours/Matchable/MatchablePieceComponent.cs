public class MatchablePieceComponent : ECSEntity, ILockerComponent
{
	public static int ComponentGuid = ECSManager.GetNextGuid();
	public override int Guid => ComponentGuid;

	private LockerComponent locker;
	public LockerComponent Locker => locker ?? GetComponent<LockerComponent>(LockerComponent.ComponentGuid);

	protected Piece context;

	public override void OnRegisterEntity(ECSEntity entity)
	{
		context = entity as Piece;

		locker = new LockerComponent();
		RegisterComponent(locker);
		
		if(context.PieceType != PieceType.Boost_CR.Id && context.Context.BoardLogic.MatchDefinition.GetNext(context.PieceType, false) == PieceType.None.Id) locker.Lock(this);
	}
    
	public virtual bool IsMatchable()
	{
		return !Locker.IsLocked;
	}
}