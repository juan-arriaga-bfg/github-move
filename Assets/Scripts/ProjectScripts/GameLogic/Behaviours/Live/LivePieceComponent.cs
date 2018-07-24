public class LivePieceComponent : IECSComponent, ILive
{
	public static int ComponentGuid = ECSManager.GetNextGuid();

	public int Guid
	{
		get { return ComponentGuid; }
	}
	
	public int HitPoints { get; set; }
	
	public int MaxHitPoints { get; set; }
    
	public virtual void OnRegisterEntity(ECSEntity entity)
	{
	}

	public virtual void OnUnRegisterEntity(ECSEntity entity)
	{
	}
	
	public bool IsLive(BoardPosition at)
	{
		return HitPoints > 0;
	}
}