﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FireflyLogicComponent : ECSEntity, IECSSystem, ILockerComponent
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	public override int Guid => ComponentGuid;
	
	private LockerComponent locker;
	public LockerComponent Locker => locker ?? GetComponent<LockerComponent>(LockerComponent.ComponentGuid);
	
	private BoardLogicComponent context;

	private List<Vector2> slots = new List<Vector2>();
    
	private readonly List<FireflyView> views = new List<FireflyView>();
    public List<FireflyView> Views => views;
	
	private int index = 1;
	private int delay = 1;
	
	private DateTime startTime = DateTime.UtcNow;
	private DateTime pauseTime = DateTime.UtcNow;
	
	private Vector2 bottom;
	private Vector2 right;
	
	private bool isClick;
	private bool isFirst = true;
	private bool isTuttorialActive => ProfileService.Current.GetStorageItem(Currency.Firefly.Name).Amount < 3;
	
	public override void OnRegisterEntity(ECSEntity entity)
	{
		context = entity as BoardLogicComponent;
		
		locker = new LockerComponent();
		RegisterComponent(locker);
		
		Locker.Lock(this);
		
		UIService.Get.OnShowWindowEvent += OnShowWindow;
		UIService.Get.OnCloseWindowEvent += OnCloseWindow;

		if (isTuttorialActive) ShopService.Current.OnPurchasedEvent += UpdateFirefly;
		
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
		
		OnMatch();
	}

	public override void OnUnRegisterEntity(ECSEntity entity)
	{
		UIService.Get.OnShowWindowEvent -= OnShowWindow;
		UIService.Get.OnCloseWindowEvent -= OnCloseWindow;
		ShopService.Current.OnPurchasedEvent -= UpdateFirefly;
	}
	
	private void OnShowWindow(IWUIWindow window)
	{
		if(UIWindowType.IsIgnore(window.WindowName)) return;

		if (Locker.IsLocked == false)
		{
			pauseTime = DateTime.UtcNow;
			
			foreach (var view in views)
			{
				view.OnDragStart();
			}
		}
		
		Locker.Lock(this);
	}
	
	private void OnCloseWindow(IWUIWindow window)
	{
		if(UIWindowType.IsIgnore(window.WindowName)) return;
		
		Locker.Unlock(this);
		
		if(Locker.IsLocked) return;
		
		foreach (var view in views)
		{
			view.OnDragEnd();
		}
		
		startTime = startTime.AddSeconds((DateTime.UtcNow - pauseTime).TotalSeconds);
	}

	private void UpdateFirefly(IPurchaseableItem purchaseableItem, IShopItem shopItem)
	{
		if (shopItem.ItemUid != Currency.Firefly.Name || isTuttorialActive) return;

		foreach (var view in views)
		{
			view.RemoveArrow();
		}
		
		ShopService.Current.OnPurchasedEvent -= UpdateFirefly;
	}
	
	public void Execute()
	{
		var amount = Random.Range(GameDataService.Current.ConstantsManager.MinAmountFirefly, GameDataService.Current.ConstantsManager.MaxAmountFirefly + 1);
		
		slots.Shuffle();
		
		for (var i = 0; i < amount; i++)
		{
			var positionFinish = new Vector2(Screen.width, 0);
			
			if (Random.Range(0, 2) == 0) positionFinish.y = Random.Range(0, Screen.height / 2f);
			else positionFinish.x = Random.Range(2 * Screen.width / 3f, Screen.width);
			
			Vector2 start = context.Context.BoardDef.ViewCamera.ScreenToWorldPoint(slots[i]);
			Vector2 finish = context.Context.BoardDef.ViewCamera.ScreenToWorldPoint(positionFinish);
			
			var firefly = context.Context.RendererContext.CreateBoardElement<FireflyView>((int) ViewType.Firefly);
			firefly.Init(context.Context.RendererContext, start, finish);
			
			views.Add(firefly);
		}
		
		if(isTuttorialActive) views[0].AddArrow();
	}
	
	public bool IsExecuteable()
	{
		if(Locker.IsLocked || views.Count > 0 || startTime.GetTime().TotalSeconds < delay) return false;
		
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

	public void Remove(FireflyView view)
	{
		views.Remove(view);
		startTime = DateTime.UtcNow;
		
		if(isTuttorialActive && views.Count != 0) views[0].AddArrow();
	}
	
	public Vector2 Cross(Vector2 a, Vector2 b) //точки a и b концы первого отрезка
	{
		Vector2 T;

		if (isFirst)
		{
			isFirst = false;
			
			bottom = context.Context.BoardDef.GetWorldPosition(context.Context.BoardDef.Width + 5, 0);
			right = context.Context.BoardDef.GetWorldPosition(context.Context.BoardDef.Width + 5, context.Context.BoardDef.Height + 5);
		}
		
		T.x = -((a.x * b.y - b.x * a.y) * (right.x - bottom.x) - (bottom.x * right.y - right.x * bottom.y) * (b.x - a.x)) / ((a.y - b.y) * (right.x - bottom.x) - (bottom.y - right.y) * (b.x - a.x));
		T.y = ((bottom.y - right.y) * (-T.x) - (bottom.x * right.y - right.x * bottom.y)) / (right.x - bottom.x);
        
		return T;
	}
}