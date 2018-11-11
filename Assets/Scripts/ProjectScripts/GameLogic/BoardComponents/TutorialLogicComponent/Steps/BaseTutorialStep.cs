public class BaseTutorialStep : ECSEntity
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	public override int Guid => ComponentGuid;

	public int Id;
	public int Repeat;
	
	public TutorialLogicComponent Context;
	
	private bool isStart;
	public bool IsPerform;
	
	public override void OnRegisterEntity(ECSEntity entity)
	{
		Context = entity as TutorialLogicComponent;
	}

	public bool IsStart()
	{
		if (isStart) return true;

		isStart = Check(TutorialConditionType.Start);
		
		return isStart;
	}

	public virtual void PauseOn()
	{
	}
	
	public virtual void Perform()
	{
		IsPerform = true;
	}
	
	public virtual void PauseOff()
	{
		if(Repeat == 0) return;
		
		Repeat--;
	}

	protected virtual void Complete()
	{
	}
	
	public bool IsComplete()
	{
		var isComplete = IsHardComplete();

		if (isComplete)
		{
			Complete();
			return true;
		}
		
		if (IsPerform == false) return false;
		
		isComplete = Check(TutorialConditionType.Complete);
			
		if (isComplete) Complete();
		
		return isComplete;
	}
	
	private bool IsHardComplete()
	{
		var collection = GetComponent<ECSComponentCollection>(BaseTutorialCondition.ComponentGuid);
		var conditions = collection?.Components.FindAll(component => (component as BaseTutorialCondition).ConditionType == TutorialConditionType.Hard);

		if (conditions == null) return false;
		
		for (var i = conditions.Count - 1; i >= 0; i--)
		{
			var condition = (BaseTutorialCondition) conditions[i];
			
			if (condition.Check() == false) continue;

			return true;
		}
		
		return false;
	}

	private bool Check(TutorialConditionType type)
	{
		var collection = GetComponent<ECSComponentCollection>(BaseTutorialCondition.ComponentGuid);
		var conditions = collection?.Components.FindAll(component => (component as BaseTutorialCondition).ConditionType == type);

		if (conditions == null) return true;
		
		for (var i = conditions.Count - 1; i >= 0; i--)
		{
			var condition = (BaseTutorialCondition) conditions[i];
			
			if (condition.Check() == false) continue;
			
			UnRegisterComponent(condition);
			conditions.Remove(condition);
		}

		return conditions.Count == 0;
	}
}