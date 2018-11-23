public class TouchReactionConditionComponent : ECSEntity
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	public override int Guid => ComponentGuid;

	public bool IsDone;
	
	
	public override void OnRegisterEntity(ECSEntity entity)
	{
		Recharge();
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