using System.Collections.Generic;
using UnityEngine;

public class WorkplaceLifeComponent : LifeComponent, IPieceBoardObserver, ILockerComponent
{
	private LockerComponent locker;
	public LockerComponent Locker => locker ?? GetComponent<LockerComponent>(LockerComponent.ComponentGuid);
	
	public virtual CurrencyPair Energy => new CurrencyPair {Currency = Currency.Energy.Name, Amount = 0};
	public virtual CurrencyPair Worker => new CurrencyPair {Currency = Currency.Worker.Name, Amount = 1};
    
	public virtual string Message => "";
	public virtual string AnalyticsLocation => "";
	public virtual string Price => string.Format(LocalizationService.Get("gameboard.bubble.button.send", "gameboard.bubble.button.send {0}"), $"<sprite name={Currency.Worker.Icon}>");

	public TimerComponent TimerWork;
	public TimerComponent TimerCooldown;
	
	public virtual TimerComponent TimerMain => TimerWork;
	
	public RewardsStoreComponent Rewards;
	
	public bool IsUseCooldown => TimerCooldown != null;
	
	protected virtual Vector2 timerOffset => new Vector2(0f, 0.5f);
	
	public float GetProgressNext => 1 - (current+1)/(float)HP;
	
	public override void OnRegisterEntity(ECSEntity entity)
	{
		base.OnRegisterEntity(entity);
		
		RegisterComponent(new LockerComponent());
		
		Rewards = new RewardsStoreComponent
		{
			GetRewards = GetRewards,
			OnComplete = OnSpawnCurrencyRewards
		};
		
		Context.RegisterComponent(Rewards);
		
		TimerWork = new TimerComponent();
		
		TimerWork.OnStart += OnTimerStart;
		TimerWork.OnComplete += OnTimerComplete;
		
		Context.RegisterComponent(TimerWork);
	}

	private void PlaySoundOnStart()
	{
	}

	private void PlaySoundOnEnd()
	{
		NSAudioService.Current.Play(SoundId.WorkerFinish);
	}
	
	public virtual void OnAddToBoard(BoardPosition position, Piece context = null)
	{
		InitInSave(position);
	}
	
	public void OnMovedFromToStart(BoardPosition from, BoardPosition to, Piece context = null)
	{
	}
	
	public virtual void OnMovedFromToFinish(BoardPosition from, BoardPosition to, Piece context = null)
	{
		Context.Context.WorkerLogic.Replace(from, to);
	}
	
	public virtual void OnRemoveFromBoard(BoardPosition position, Piece context = null)
	{
		TimerWork.OnStart -= OnTimerStart;
		TimerWork.OnComplete -= OnTimerComplete;
	}

	protected virtual LifeSaveItem InitInSave(BoardPosition position)
	{
		Rewards.InitInSave(position);
		
		var save = ProfileService.Current.GetComponent<FieldDefComponent>(FieldDefComponent.ComponentGuid);
		var item = save?.GetLifeSave(position);
		
		if(item == null) return null;
		
		current = item.Step;
		Context.Context.WorkerLogic.Init(Context.CachedPosition, TimerWork);

		if (item.IsStartTimer) TimerWork.Start(item.StartTimeTimer);
		else OnTimerStart();

		return item;
	}

	public virtual LifeSaveItem Save()
	{
		return current == 0 ? null : new LifeSaveItem
		{
			Step = current,
			Position = Context.CachedPosition,
			IsStartTimer = TimerWork.IsExecuteable(),
			StartTimeTimer = TimerWork.StartTimeLong
		};
	}
    
	public virtual bool Damage(bool isExtra = false)
	{
		if (IsDead
		    || CurrencyHelper.IsCanPurchase(Energy, true) == false
		    || isExtra == false && Context.Context.WorkerLogic.Get(Context.CachedPosition, TimerWork) == false) return false;
        
		CurrencyHelper.Purchase(Currency.Damage.Name, 1, Energy, success =>
		{
			Success();
			Damage(Worker?.Amount ?? 1);
			TimerWork.Start();
            
			BoardService.Current.FirstBoard.BoardEvents.RaiseEvent(GameEventsCodes.StorageDamage, this);
		});
        
		return true;
	}
	
	protected void OnTimerStart()
	{
		if (IsDead == false) OnStep();
		else OnComplete();
        
		PlaySoundOnStart();
		
		Locker.Lock(this, false);

		if (TimerWork.IsExecuteable()) UpdateView(true);
	}
	
	protected virtual void OnTimerComplete()
	{
		Context.Context.WorkerLogic.Return(Context.CachedPosition);
		PlaySoundOnEnd();
		UpdateView(false);
		Rewards.ShowBubble();
	}
    
	protected virtual Dictionary<int, int> GetRewards()
	{
		return new Dictionary<int, int>();
	}
	
	protected virtual void Success()
	{
	}
	
	protected virtual void OnStep()
	{
		Rewards.IsTargetReplace = false;
	}
	
	protected virtual void OnComplete()
	{
		Rewards.IsTargetReplace = true;
	}
	
	protected virtual void OnSpawnCurrencyRewards(bool isComplete)
	{
		if(isComplete) Locker.Unlock(this);
	}
	
	private void UpdateView(bool isShow)
	{
		if(Context.ViewDefinition == null) return;
        
		var view = Context.ViewDefinition.AddView(ViewType.BoardTimer);

		if (isShow)
		{
			view.Priority = -1;
			view.Offset = timerOffset;
			view.SetOffset();
		}
		else
		{
			view.Priority = 10;
		}
		
		view.Change(isShow);
	}
}