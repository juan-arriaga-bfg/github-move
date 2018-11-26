using System;

public class BaseTutorialStep : ECSEntity
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	public override int Guid => ComponentGuid;

	public int Id;
	public int Repeat;
	
	public TutorialLogicComponent Context;
	
	private bool isStart;
	public bool IsPerform;
	public bool IsIgnoreUi;

	public bool isAnyStartCondition;
	public bool isAnyCompleteCondition;
	
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
		StartAnimation(TutorialAnimationType.Perform);
	}
	
	public virtual void PauseOff()
	{
		if(Repeat == 0) return;
		
		Repeat--;
	}

	protected virtual void Complete()
	{
		StartAnimation(TutorialAnimationType.Complete);
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
		
		var isAny = false;
		var isRemove = false;
		
		switch (type)
		{
			case TutorialConditionType.Start:
				isAny = isAnyStartCondition;
				break;
			case TutorialConditionType.Complete:
				isAny = isAnyCompleteCondition;
				break;
		}
		
		if (isAny)
		{
			isRemove = conditions.Find(component => (component as BaseTutorialCondition).Check()) != null;
		}
		
		for (var i = conditions.Count - 1; i >= 0; i--)
		{
			var condition = (BaseTutorialCondition) conditions[i];
			
			if (isRemove == false && condition.Check() == false) continue;
			
			UnRegisterComponent(condition);
			conditions.Remove(condition);
		}

		return conditions.Count == 0;
	}
	
	private void StartAnimation(TutorialAnimationType type)
	{
		var collection = GetComponent<ECSComponentCollection>(BaseTutorialAnimation.ComponentGuid);
		var animations = collection?.Components.FindAll(component => (component as BaseTutorialAnimation).AnimationType == type);

		if (animations == null) return;
		
		for (var i = animations.Count - 1; i >= 0; i--)
		{
			var animation = (BaseTutorialAnimation) animations[i];
			
			animation.Start();
			UnRegisterComponent(animation);
			animations.Remove(animation);
		}
	}
}