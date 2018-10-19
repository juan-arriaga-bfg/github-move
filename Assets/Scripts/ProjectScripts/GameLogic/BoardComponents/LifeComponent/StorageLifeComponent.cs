public class StorageLifeComponent : LifeComponent, IPieceBoardObserver, ITimerComponent, ILockerComponent
{
    private LockerComponent locker;
    public LockerComponent Locker => locker ?? GetComponent<LockerComponent>(LockerComponent.ComponentGuid);
    
    protected StorageComponent storage;
    
    public virtual CurrencyPair Energy => new CurrencyPair {Currency = Currency.Energy.Name, Amount = 0};
    public virtual CurrencyPair Worker => new CurrencyPair {Currency = Currency.Worker.Name, Amount = 1};
    
    public virtual string Message => "";
    public virtual string Price => $"Send <sprite name={Worker.Currency}>";
    
    public virtual bool IsUseCooldown => false;
    
    public virtual TimerComponent Timer => storage.Timer;
    public float GetProgressNext => 1 - (current+1)/(float)HP;

    public override void OnRegisterEntity(ECSEntity entity)
    {
        base.OnRegisterEntity(entity);
        
        locker = new LockerComponent();
        RegisterComponent(locker);
    }

    public virtual void OnAddToBoard(BoardPosition position, Piece context = null)
    {
        if(storage == null) storage = thisContext.GetComponent<StorageComponent>(StorageComponent.ComponentGuid);
        
        var timer = thisContext.GetComponent<TimerComponent>(TimerComponent.ComponentGuid);

        if (timer != null)
        {
            timer.OnStart += OnTimerStart;
            timer.OnComplete += OnTimerComplete;
        }
        
        InitInSave(position);
    }

    protected virtual LifeSaveItem InitInSave(BoardPosition position)
    {
        var save = ProfileService.Current.GetComponent<FieldDefComponent>(FieldDefComponent.ComponentGuid);

        var item = save?.GetLifeSave(position);
        
        if (item == null) return null;
        
        current = item.Step;

        thisContext.Context.WorkerLogic.Init(thisContext.CachedPosition, storage.Timer);
        OnTimerStart();

        if (storage.IsFilled) OnTimerComplete();
        
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
        storage.Timer.OnStart -= OnTimerComplete;
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