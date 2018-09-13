public class StorageLifeComponent : LifeComponent, IPieceBoardObserver, ITimerComponent
{
    protected StorageComponent storage;
    
    public virtual CurrencyPair Energy => new CurrencyPair {Currency = Currency.Energy.Name, Amount = 0};
    public virtual CurrencyPair Worker => new CurrencyPair {Currency = Currency.Worker.Name, Amount = 1};
    
    public virtual string Message => "";
    public virtual string Price => $"Send<sprite name={Worker.Currency}>";
    
    public virtual bool IsUseCooldown => false;

    public string Key => thisContext.CachedPosition.ToSaveString();

    public virtual TimerComponent Timer => storage.Timer;
    public float GetProgressNext => 1 - (current+1)/(float)HP;

    public virtual void OnAddToBoard(BoardPosition position, Piece context = null)
    {
        if(storage == null) storage = thisContext.GetComponent<StorageComponent>(StorageComponent.ComponentGuid);
        
        var timer = thisContext.GetComponent<TimerComponent>(TimerComponent.ComponentGuid);

        if (timer != null) timer.OnStart += OnTimerStart;
        
        InitInSave(position);
    }

    protected virtual LifeSaveItem InitInSave(BoardPosition position)
    {
        var save = ProfileService.Current.GetComponent<FieldDefComponent>(FieldDefComponent.ComponentGuid);

        var item = save?.GetLifeSave(position);
        
        if (item == null) return null;
        
        current = item.Step;
        
        
        
        OnTimerStart();

        return item;
    }

    public void OnMovedFromToStart(BoardPosition @from, BoardPosition to, Piece context = null)
    {
    }

    public virtual void OnMovedFromToFinish(BoardPosition from, BoardPosition to, Piece context = null)
    {
        thisContext.Context.WorkerLogic.Replase(from.ToSaveString(), to.ToSaveString());
    }

    public virtual void OnRemoveFromBoard(BoardPosition position, Piece context = null)
    {
        storage.Timer.OnStart -= OnTimerStart;
    }

    protected virtual int GetTimerDelay()
    {
        return storage.Timer.Delay;
    }
    
    public virtual bool Damage()
    {
        if (IsDead) return false;
        
        var isSuccess = false;

        if (CurrencyHellper.IsCanPurchase(Energy, true) == false
            || thisContext.Context.WorkerLogic.Get(Key, GetTimerDelay()) == false) return false;
        
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
    }
}