﻿using System;
using System.Collections.Generic;
using UnityEngine;

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
	public bool IsIgnoreDebug = true;

	public bool IsAnyStartCondition;
	public bool IsAnyCompleteCondition;

	protected bool isPauseOn;
	protected bool isAutoComplete;
	
	public Action OnFirstStartCallback;
	public Action OnCompleteCallback;
	
	public override void OnRegisterEntity(ECSEntity entity)
	{
		Context = entity as TutorialLogicComponent;
	}

	public override void OnUnRegisterEntity(ECSEntity entity)
	{
		base.OnUnRegisterEntity(entity);
	}

	public bool IsStart()
	{
		if (isStart) return true;

		isStart = Check(TutorialConditionType.Start);
		
		return isStart;
	}
	
	public virtual void PauseOn()
	{
		isPauseOn = true;
	}

	protected virtual void OnFirstStart()
	{
		var tutorialLogic = BoardService.Current.FirstBoard.TutorialLogic;
		var started = tutorialLogic.SaveStarted;
		started.Add(Id);
		OnFirstStartCallback?.Invoke();
	}
	
	public virtual void Perform()
	{
		if (IsPerform == false)
		{
			var tutorialLogic = BoardService.Current.FirstBoard.TutorialLogic;
			var started = tutorialLogic.SaveStarted;
			if (started.Contains(Id) == false)
			{
				OnFirstStart();
			}
		}
		IsPerform = true;
		StartAnimation(TutorialAnimationType.Perform);
	}
	
	public virtual void PauseOff()
	{
		isPauseOn = false;
		
		if(Repeat == 0) return;
		
		Repeat--;
	}

	protected virtual void Complete()
	{
		OnCompleteCallback?.Invoke();
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
		
#if DEBUG
		if (isComplete == false && IsIgnoreDebug) isComplete = !DevTools.IsTutorialEnabled();
#endif
		
		if (isComplete) Complete();
		
		return isComplete;
	}
	
	private bool IsHardComplete()
	{
		if (isAutoComplete) return true;
		
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
				isAny = IsAnyStartCondition;
				break;
			case TutorialConditionType.Complete:
				isAny = IsAnyCompleteCondition;
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