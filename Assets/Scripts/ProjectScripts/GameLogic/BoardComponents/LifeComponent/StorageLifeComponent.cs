using System;
using System.Collections.Generic;

public class StorageLifeComponent : LifeComponent, IPieceBoardObserver, ITimerComponent, ILockerComponent
{
    private LockerComponent locker;
    public LockerComponent Locker => locker ?? GetComponent<LockerComponent>(LockerComponent.ComponentGuid);
    
    protected StorageComponent storage;
    
    public virtual CurrencyPair Energy => new CurrencyPair {Currency = Currency.Energy.Name, Amount = 0};
    public virtual CurrencyPair Worker => new CurrencyPair {Currency = Currency.Worker.Name, Amount = 1};
    
    public virtual string Message => "";
    public virtual string Price => string.Format(LocalizationService.Get("gameboard.bubble.button.send", "gameboard.bubble.button.send {0}"), $"<sprite name={Currency.Worker.Name}>");
    
    public virtual bool IsUseCooldown => false;
    public bool IsFilled => storage.IsFilled;
    
    public virtual TimerComponent Timer => storage.Timer;
    public float GetProgressNext => 1 - (current+1)/(float)HP;

    public Dictionary<int, int> Reward;
    public virtual int StorageSpawnPiece => storage.SpawnPiece;
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        base.OnRegisterEntity(entity);
        
        locker = new LockerComponent();
        RegisterComponent(locker);
        
        storage = thisContext.GetComponent<StorageComponent>(StorageComponent.ComponentGuid);
        
        if (storage.Timer == null) return;
        
        storage.Timer.OnStart += OnTimerStart;
        storage.Timer.OnComplete += OnTimerComplete;
    }

    public virtual void OnAddToBoard(BoardPosition position, Piece context = null)
    {
        InitStorage();
        InitInSave(position);
    }

    protected virtual void InitStorage()
    {
    }

    protected virtual Action InitInSaveStorage(LifeSaveItem item)
    {
        Action updateView;
        storage.InitInSave(thisContext.CachedPosition, () => InitInSaveReward(item), out updateView);
        return updateView;
    }
    
    protected virtual void InitInSaveReward(LifeSaveItem item)
    {
        Reward = item.Reward;
    }

    protected virtual LifeSaveItem InitInSave(BoardPosition position)
    {
        var save = ProfileService.Current.GetComponent<FieldDefComponent>(FieldDefComponent.ComponentGuid);

        var item = save?.GetLifeSave(position);
        
        if (item == null) return null;
        
        current = item.Step;
        thisContext.Context.WorkerLogic.Init(thisContext.CachedPosition, storage.Timer);
        
        var updateView = InitInSaveStorage(item);
        
        if (storage.IsFilled)
        {
            OnTimerStart();
            OnTimerComplete();
        }
        
        updateView?.Invoke();
        
        return item;
    }

    public void OnMovedFromToStart(BoardPosition @from, BoardPosition to, Piece context = null)
    {
    }

    public virtual void OnMovedFromToFinish(BoardPosition from, BoardPosition to, Piece context = null)
    {
        thisContext.Context.WorkerLogic.Replace(from, to);
    }

    public virtual void OnRemoveFromBoard(BoardPosition position, Piece context = null)
    {
        storage.Timer.OnStart -= OnTimerStart;
        storage.Timer.OnComplete -= OnTimerComplete;
    }
    
    public virtual bool Damage(bool isExtra = false)
    {
        if (IsDead) return false;
        
        var isSuccess = false;

        if (CurrencyHellper.IsCanPurchase(Energy, true) == false
            || isExtra == false && thisContext.Context.WorkerLogic.Get(thisContext.CachedPosition, storage.Timer) == false) return false;
        
        Success();
        
        CurrencyHellper.Purchase(Currency.Damage.Name, 1, Energy, success =>
        {
            if(success == false) return;
            
            isSuccess = true;
            
            Damage(Worker?.Amount ?? 1);
            
            storage.Timer.Start();
            
            BoardService.Current.FirstBoard.BoardEvents.RaiseEvent(GameEventsCodes.StorageDamage, this);
        });
        
        return isSuccess;
    }

    private void OnTimerStart()
    {
        if (IsDead == false) OnStep();
        else OnComplete();
        
        locker.Lock(this, false);
    }
    
    protected virtual void Success()
    {
    }

    protected virtual void OnStep()
    {
    }
    
    protected virtual void OnComplete()
    {
    }

    protected virtual void OnSpawnRewards()
    {
        locker.Unlock(this);
    }
    
    protected virtual void OnTimerComplete()
    {
        thisContext.Context.WorkerLogic.Return(thisContext.CachedPosition);
    }
}