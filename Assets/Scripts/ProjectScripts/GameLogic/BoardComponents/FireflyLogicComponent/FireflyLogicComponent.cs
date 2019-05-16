using System;
using System.Collections.Generic;
using BfgAnalytics;
using UnityEngine;
using Random = UnityEngine.Random;

public class FireflyLogicComponent : ECSEntity, IECSSystem, ILockerComponent, ITouchableBoardObjectLogic
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	public override int Guid => ComponentGuid;
	
	private LockerComponent locker;
	public LockerComponent Locker => locker ?? GetComponent<LockerComponent>(LockerComponent.ComponentGuid);
	
	private BoardLogicComponent context;
	private FireflyDef Def;
	
	private readonly List<Vector2> slots = new List<Vector2>();
	private readonly List<FireflyView> views = new List<FireflyView>();
    public List<FireflyView> Views => views;
	
	private int index = 1;
	private int delay = 1;

	private DateTime tutorialStartTime;
	
	private DateTime startTime;
	private DateTime pauseTime;
	
	private Vector2 bottom;
	private Vector2 right;
	
	private bool isClick;

	private const int TUTORIAL_FIREFLY_COUNT = 3;
	private bool isTutorialActive => ProfileService.Current.GetStorageItem(Currency.Firefly.Name).Amount < TUTORIAL_FIREFLY_COUNT;

	private TimerComponent restartTimer;
	
	public bool IsDraggable => true;
	
	public override void OnRegisterEntity(ECSEntity entity)
	{
		context = entity as BoardLogicComponent;

        startTime = SecuredTimeService.Current.UtcNow;
        pauseTime = startTime;
        tutorialStartTime = DateTime.UtcNow;
        
		locker = new LockerComponent();
		RegisterComponent(locker);
		Locker.Lock(this);

		restartTimer = new TimerComponent
		{
			OnComplete = () =>
			{
				index = 1;
				isClick = false;
				OnMatch();
				restartTimer.Start();
				IW.Logger.Log($"[FireflyLogicComponent] => restart session!");
			}
		};
		
		RegisterComponent(restartTimer);
		
		UIService.Get.OnShowWindowEvent += OnShowWindow;
		UIService.Get.OnCloseWindowEvent += OnCloseWindow;

		if (isTutorialActive) ShopService.Current.OnPurchasedEvent += UpdateFirefly;
		
		const int step = 100;
		
		for (var i = 0; i < 5; i++)
		{
			var y1 = Screen.height + 50 + step * i;
			
			for (var x1 = -50; x1 < Screen.width / 3f; x1 += step)
			{
				slots.Add(new Vector2(x1, y1));
			}
			
			var x2 = -50 - step * i;
			
			for (var y2 = Screen.height / 2f; y2 < Screen.height + 50; y2 += step)
			{
				slots.Add(new Vector2(x2, y2));
			}
		}
		
		bottom = context.Context.BoardDef.GetWorldPosition(context.Context.BoardDef.Width + 5, 0);
		right = context.Context.BoardDef.GetWorldPosition(context.Context.BoardDef.Width + 5, context.Context.BoardDef.Height + 5);
		
		var isEventGameActive = context.EventGamesLogic.GetEventGame(EventGameType.OrderSoftLaunch, out var eventGame) && eventGame.State == EventGameState.InProgress;

		ResetLogic(isEventGameActive ? FireflyLogicType.Event : FireflyLogicType.Normal);
		
		context.EventGamesLogic.OnStart += OnStartEvent;
		context.EventGamesLogic.OnStop += OnStopEvent;
	}
	
	public override void OnUnRegisterEntity(ECSEntity entity)
	{
		UIService.Get.OnShowWindowEvent -= OnShowWindow;
		UIService.Get.OnCloseWindowEvent -= OnCloseWindow;
		ShopService.Current.OnPurchasedEvent -= UpdateFirefly;
		
		context.EventGamesLogic.OnStart -= OnStartEvent;
		context.EventGamesLogic.OnStop -= OnStopEvent;

		restartTimer.OnComplete = null;
	}

	public void ResetTutorialStartTime()
	{
		tutorialStartTime = DateTime.UtcNow;
	}

	public void ResetSession()
	{
		restartTimer.Reset();
	}

	private void OnStartEvent(EventGameType name)
	{
		if (name != EventGameType.OrderSoftLaunch) return;
		
		ResetLogic(FireflyLogicType.Event);
	}

	private void OnStopEvent(EventGameType name)
	{
		if (name != EventGameType.OrderSoftLaunch) return;

		DestroyAll(FireflyType.Event);
		ResetLogic(FireflyLogicType.Normal);
	}

	public void ResetLogic(FireflyLogicType logicType)
	{
		Def = GameDataService.Current.FireflyManager.Defs[logicType];

		restartTimer.Delay = Def.SleepDelay;
		
		delay = Def.DelayFirstSpawn.Range();
		startTime = SecuredTimeService.Current.UtcNow;
		
		ResetSession();
	}
	
	private void OnShowWindow(IWUIWindow window)
	{
		if(UIWindowType.IsIgnore(window.WindowName)) return;

		if (Locker.IsLocked == false)
		{
			pauseTime = SecuredTimeService.Current.UtcNow;
			
			foreach (var view in views)
			{
				view.StopFly();
			}
		}
		
		Locker.Lock(this);
	}
	
	private void OnCloseWindow(IWUIWindow window)
	{
		if (UIWindowType.IsIgnore(window.WindowName)) return;
		
		Locker.Unlock(this);

		if (Locker.IsLocked) return;
		
		foreach (var view in views)
		{
			view.StartFly();
		}

		var seconds = (int)(SecuredTimeService.Current.UtcNow - pauseTime).TotalSeconds;
		
		restartTimer.Add(seconds);
		startTime = startTime.AddSeconds(seconds);
	}

	private void UpdateFirefly(IPurchaseableItem purchaseableItem, IShopItem shopItem)
	{
		if (shopItem.ItemUid != Currency.Firefly.Name || isTutorialActive) return;

		foreach (var view in views)
		{
			view.RemoveArrow();
		}
		
		ShopService.Current.OnPurchasedEvent -= UpdateFirefly;
	}
	
	public void Execute()
	{
		var amount = Def.AmountProduction.Range() + Def.AmountEvent.Range();
		
		slots.Shuffle();
		
		for (var i = 0; i < amount; i++)
		{
			var positionFinish = new Vector2(Screen.width, 0);
			
			if (Random.Range(0, 2) == 0) positionFinish.y = Random.Range(0, Screen.height / 2f);
			else positionFinish.x = Random.Range(2 * Screen.width / 3f, Screen.width);
			
			Vector2 start = context.Context.BoardDef.ViewCamera.ScreenToWorldPoint(slots[i]);
			Vector2 finish = context.Context.BoardDef.ViewCamera.ScreenToWorldPoint(positionFinish);

			var firefly = (int) (i < Def.AmountProduction.Value ? ViewType.FireflyProduction : ViewType.FireflyEvent);
			var view = context.Context.RendererContext.CreateBoardElement<FireflyView>(firefly);
			view.Init(context.Context.RendererContext, start, finish);
			
			views.Add(view);
		}

		if (!isTutorialActive) return;
		
		views[0].AddArrow();
		ResetTutorialStartTime();
	}
	
	public bool IsExecuteable()
	{
		if (Locker.IsLocked || views.Count > 0 || startTime.GetTime().TotalSeconds < delay) return false;
		
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
			delay = Def.TapDelay * index;
			startTime = SecuredTimeService.Current.UtcNow;
			index++;
		}
		
		var firefly = view as FireflyView;
		
		if (firefly == null) return false;

		var amountFireflyBefore = ProfileService.Current.GetStorageItem(Currency.Firefly.Name).Amount;
		
		firefly.OnClick();
		
		var amountFireflyAfter = ProfileService.Current.GetStorageItem(Currency.Firefly.Name).Amount;

		if (amountFireflyAfter != amountFireflyBefore && amountFireflyAfter == TUTORIAL_FIREFLY_COUNT)
		{
			Analytics.SendTutorialEndStepEvent("firefly", tutorialStartTime);
		}
		
		return true;
	}

	public void OnMatch()
	{
		if (isClick) return;

		delay = Def.DelaySpawn.Range();
		startTime = SecuredTimeService.Current.UtcNow;
	}

	public void Remove(FireflyView view)
	{
		views.Remove(view);
		startTime = SecuredTimeService.Current.UtcNow;
		
		if(isTutorialActive && views.Count != 0) views[0].AddArrow();
	}

	public void DestroyAll(FireflyType name)
	{
		for (var i = views.Count - 1; i >= 0; i--)
		{
			var view = views[i];
			
			if (view.FireflyType != name) continue;
			
			views.Remove(view);
			context.Context.RendererContext.DestroyElement(view.gameObject);
		}
	}

	public void DestroyAll()
	{
		for (var i = views.Count - 1; i >= 0; i--)
		{
			var view = views[i];
			views.Remove(view);
			context.Context.RendererContext.DestroyElement(view.gameObject);
		}
	}
	
	public Vector2 Cross(Vector2 a, Vector2 b) //точки a и b концы первого отрезка
	{
		IW.Logger.Log($"[Firefly] => a: {a}, b: {b}, right: {right}, bottom: {bottom}");
		
		var x = -((a.x * b.y - b.x * a.y) * (right.x - bottom.x) - (bottom.x * right.y - right.x * bottom.y) * (b.x - a.x)) / ((a.y - b.y) * (right.x - bottom.x) - (bottom.y - right.y) * (b.x - a.x));
		var y = ((bottom.y - right.y) * (-x) - (bottom.x * right.y - right.x * bottom.y)) / (right.x - bottom.x);
		
		return new Vector2(x, y);
	}
}