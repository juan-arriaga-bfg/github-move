public enum TutorialAnimationType
{
	Perform,
	Complete
}

public class BaseTutorialAnimation : IECSComponent
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	public int Guid => ComponentGuid;
	
	public TutorialAnimationType AnimationType;
	
	protected BaseTutorialStep context;

	protected bool isStart;
	
	public virtual void OnRegisterEntity(ECSEntity entity)
	{
		context = entity as BaseTutorialStep;
	}
	
	public virtual void OnUnRegisterEntity(ECSEntity entity)
	{
	}
	
	public virtual void Start()
	{
		isStart = true;
	}
}