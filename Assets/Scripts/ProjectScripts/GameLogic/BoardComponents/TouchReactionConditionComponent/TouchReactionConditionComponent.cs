public class TouchReactionConditionComponent : ECSEntity, ILockerComponent
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	public override int Guid => ComponentGuid;

	public bool IsDone;
	
	private LockerComponent locker;
	public LockerComponent Locker => locker ?? GetComponent<LockerComponent>(LockerComponent.ComponentGuid);
	
	public override void OnRegisterEntity(ECSEntity entity)
	{
		RegisterComponent(new LockerComponent());
		Recharge();
	}
	
	public virtual void Recharge()
	{
		IsDone = false;
	}
	
	public virtual bool Check(BoardPosition position, Piece piece)
	{
		IsDone = true;
		return IsDone && !Locker.IsLocked;
	}
}