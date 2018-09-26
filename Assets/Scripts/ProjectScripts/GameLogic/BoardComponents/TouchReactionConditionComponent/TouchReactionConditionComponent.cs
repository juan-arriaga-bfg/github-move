public class TouchReactionConditionComponent : ECSEntity
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	public override int Guid => ComponentGuid;

	public bool IsDone;
	
	private LockerComponent locker;
	public LockerComponent Locker => locker ?? GetComponent<LockerComponent>(LockerComponent.ComponentGuid);
	
	public override void OnRegisterEntity(ECSEntity entity)
	{
		if (Locker == null)
		{
			locker = new LockerComponent();
			RegisterComponent(locker);
		}
			
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