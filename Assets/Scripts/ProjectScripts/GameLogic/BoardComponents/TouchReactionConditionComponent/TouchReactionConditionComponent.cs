public class TouchReactionConditionComponent : IECSComponent
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	public int Guid => ComponentGuid;

	public bool IsDone;
	
	public virtual void OnRegisterEntity(ECSEntity entity)
	{
		Recharge();
	}
	
	public void OnUnRegisterEntity(ECSEntity entity)
	{
	}
	
	public virtual void Recharge()
	{
		IsDone = false;
	}
	
	public virtual bool Check(BoardPosition position, Piece piece)
	{
		IsDone = true;
		return IsDone;
	}
}