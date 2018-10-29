using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FireflyLogicComponent : IECSComponent, IECSSystem
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	public int Guid => ComponentGuid;

	private BoardLogicComponent context;

	private int delay = 1;
	private DateTime startTime;
	
	public void OnRegisterEntity(ECSEntity entity)
	{
		context = entity as BoardLogicComponent;
		startTime = DateTime.UtcNow;
	}

	public void OnUnRegisterEntity(ECSEntity entity)
	{
	}
	
	public void Execute()
	{
		var firefly = context.Context.RendererContext.CreateBoardElement<FireflyView>((int) ViewType.Firefly);
		
		firefly.GetComponent<FireflyView>().Init(context.Context.RendererContext);
	}

	public bool IsExecuteable()
	{
		if(startTime.GetTime().TotalSeconds < delay) return false;
		
		startTime = DateTime.UtcNow;
		return true;
	}
	
	public object GetDependency()
	{
		return null;
	}

	public bool ChecK(BoardElementView view)
	{
		var firefly = view as FireflyView;

		return firefly != null;
	}

	public bool OnDragStart(BoardElementView view)
	{
		var firefly = view as FireflyView;

		if (firefly == null) return false;
		
		firefly.OnDragStart();
		return true;
	}

	public bool OnDragEnd(BoardElementView view)
	{
		var firefly = view as FireflyView;

		if (firefly == null) return false;
		
		firefly.OnDragEnd();
		return true;
	}

	public bool OnClick(BoardElementView view)
	{
		var firefly = view as FireflyView;
		
		if (firefly == null) return false;
		
		firefly.OnClick();
		return true;
	}
}