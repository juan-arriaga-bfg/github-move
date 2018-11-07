using System;
using Random = UnityEngine.Random;

public class FireflyLogicComponent : IECSComponent, IECSSystem
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	public int Guid => ComponentGuid;

	private BoardLogicComponent context;

	private int amount;
	private int index = 1;
	private int delay = 1;
	
	private DateTime startTime;

	private bool isClick;
	
	public void OnRegisterEntity(ECSEntity entity)
	{
		context = entity as BoardLogicComponent;
		OnMatch();
	}

	public void OnUnRegisterEntity(ECSEntity entity)
	{
	}
	
	public void Execute()
	{
		amount = Random.Range(GameDataService.Current.ConstantsManager.MinAmountFirefly, GameDataService.Current.ConstantsManager.MaxAmountFirefly + 1);
		
		for (var i = 0; i < amount; i++)
		{
			var firefly = context.Context.RendererContext.CreateBoardElement<FireflyView>((int) ViewType.Firefly);
			firefly.Init(context.Context.RendererContext);
		}
	}
	
	public bool IsExecuteable()
	{
		if(amount > 0 || startTime.GetTime().TotalSeconds < delay) return false;
		
		isClick = false;
		OnMatch();
		return true;
	}
	
	public object GetDependency()
	{
		return null;
	}

	public bool Check(BoardElementView view)
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
		if (isClick == false)
		{
			isClick = true;
			delay = GameDataService.Current.ConstantsManager.TapDelayFirefly * index;
			startTime = DateTime.UtcNow;
			index++;
		}
		
		var firefly = view as FireflyView;
		
		if (firefly == null) return false;
		
		firefly.OnClick();
		return true;
	}

	public void OnMatch()
	{
		if (isClick) return;

		delay = Random.Range(GameDataService.Current.ConstantsManager.MinDelaySpawnFirefly, GameDataService.Current.ConstantsManager.MaxDelaySpawnFirefly + 1);
		startTime = DateTime.UtcNow;
	}

	public void Remove()
	{
		amount--;
		startTime = DateTime.UtcNow;
	}
}