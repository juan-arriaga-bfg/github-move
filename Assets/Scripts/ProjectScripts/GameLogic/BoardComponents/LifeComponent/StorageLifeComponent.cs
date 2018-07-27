using System.Collections.Generic;

public class StorageLifeComponent : LifeComponent, IPieceBoardObserver
{
    protected StorageComponent storage;
    
    public virtual CurrencyPair Energy
    {
        get { return new CurrencyPair(); }
    }
    
    public virtual CurrencyPair Worker
    {
        get { return new CurrencyPair {Currency = Currency.Worker.Name, Amount = 1}; }
    }
    
    public virtual string Key
    {
        get { return thisContext.CachedPosition.ToSaveString(); }
    }
    
    public virtual void OnAddToBoard(BoardPosition position, Piece context = null)
    {
        if(storage == null) storage = thisContext.GetComponent<StorageComponent>(StorageComponent.ComponentGuid);
        
        var timer = thisContext.GetComponent<TimerComponent>(TimerComponent.ComponentGuid);

        if (timer != null) timer.OnStart += OnTimerStart;
        
        var save = ProfileService.Current.GetComponent<FieldDefComponent>(FieldDefComponent.ComponentGuid);

        if (save == null) return;
        
        var item = save.GetLifeSave(position);
        
        if (item == null) return;
        
        current = item.Step;
        
        OnTimerStart();
    }

    public virtual void OnMovedFromTo(BoardPosition from, BoardPosition to, Piece context = null)
    {
    }

    public virtual void OnRemoveFromBoard(BoardPosition position, Piece context = null)
    {
        storage.Timer.OnStart -= OnTimerStart;
    }
    
    public bool Damage()
    {
        if (current == HP) return false;
        
        var isSuccess = false;

        if (CurrencyHellper.IsCanPurchase(Energy, true) == false
            || thisContext.Context.WorkerLogic.Get(Key, storage.Timer.Delay) == false) return false;
        
        Success();
        
        CurrencyHellper.Purchase(Currency.Damage.Name, 1, Energy, success =>
        {
            if(success == false) return;
            
            isSuccess = true;
            
            Damage(Worker == null ? 1 : Worker.Amount);
            
            storage.Timer.Start();
        });
        
        return isSuccess;
    }

    private void OnTimerStart()
    {
        if (current != HP) OnStep();
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
}