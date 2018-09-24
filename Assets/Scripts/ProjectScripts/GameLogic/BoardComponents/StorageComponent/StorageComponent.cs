using System;
using UnityEngine;

public class StorageComponent : IECSComponent, ITimerComponent, IPieceBoardObserver
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;

    public IBoardAction SpawnAction;
    public int SpawnPiece;
    
    public int Amount;
    public int Capacity;
    public int Filling;

    public bool isSpawnResource => string.IsNullOrEmpty(PieceType.Parse(SpawnPiece));
    public string Icon => isSpawnResource ? $"icon_{Currency.GetCurrencyDef(SpawnPiece).Name}" : PieceType.Parse(SpawnPiece);

    public bool IsTimerShow;
    public bool IsAutoStart = true;

    public bool IsFilled => Filling == Capacity;
    
    public Action OnHideBubble;
    
    public TimerComponent Timer { get; private set; }
    public Vector2 TimerOffset = Vector2.zero;

    public Action OnScatter;
    
    private Piece pieceContext;
    
    public void OnRegisterEntity(ECSEntity entity)
    {
        pieceContext = entity as Piece;
    }
    
    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }
    
    public void OnAddToBoard(BoardPosition position, Piece context = null)
    {
        Timer = pieceContext.GetComponent<TimerComponent>(TimerComponent.ComponentGuid);
        
        if(Timer == null) return;
        
        Timer.OnComplete += Update;

        if (IsTimerShow)
        {
            Timer.OnStart += OnShowTimer;
            Timer.OnComplete += OnHideTimer;
        }
        
        if (InitInSave(position) == false && IsAutoStart && !IsFilled)
        {
            Timer.Start();
        }
        
        UpdateView();
    }
    
    private bool InitInSave(BoardPosition position)
    {
        var save = ProfileService.Current.GetComponent<FieldDefComponent>(FieldDefComponent.ComponentGuid);

        var item = save?.GetStorageSave(position);

        if (item == null) return false;

        if (item.IsStart == false)
        {
            item.StartTime = DateTime.UtcNow.ConvertToUnixTime();
            Filling = Mathf.Min(item.Filling, Capacity);
            return true;
        }
        
        DateTime now;
        var steps = DateTimeExtension.CountOfStepsPassedWhenAppWasInBackground(item.StartTime, Timer.Delay, out now);
        
        Filling = Mathf.Min(item.Filling + Mathf.Max(steps, 0), Capacity);
        item.StartTime = now.ConvertToUnixTime();
        
        if (item.IsStart && !IsFilled) Timer.Start(item.StartTime);
        
        return true;
    }
    
    public void OnMovedFromToStart(BoardPosition @from, BoardPosition to, Piece context = null)
    {
    }

    public void OnMovedFromToFinish(BoardPosition @from, BoardPosition to, Piece context = null)
    {
    }

    public void OnRemoveFromBoard(BoardPosition position, Piece context = null)
    {
        if(Timer == null) return;
        
        Timer.Stop();
        
        Timer.OnComplete -= Update;
        
        if(IsTimerShow == false) return;

        Timer.OnStart -= OnShowTimer;
        Timer.OnComplete -= OnHideTimer;
    }

    private void Update()
    {
        Filling = Mathf.Min(Filling + Amount, Capacity);
        
        if(!IsFilled) Timer.Start();
        
        UpdateView();
    }
    
    private void UpdateView()
    {
        if(pieceContext.ViewDefinition == null) return;
        
        var view = pieceContext.ViewDefinition.AddView(ViewType.StorageState);
        var isShow = Filling / (float) Capacity > 0.2f;
        
        view.Change(isShow);

        if (isShow)
        {
            pieceContext.Context.BoardEvents.RaiseEvent(GameEventsCodes.ClosePieceUI, this);
            pieceContext.Context.HintCooldown.AddView(view);
            return;
        }

        view.OnHide = OnHideBubble;
        pieceContext.Context.HintCooldown.RemoweView(view);
    }

    public bool Scatter(out int amount, bool isStartNext = true)
    {   
        amount = Filling;
        
        if (Filling == 0) return false;
        
        Filling = 0;
        if(isStartNext) Timer.Start();
        UpdateView();
        
        return true;
    }

    private void OnShowTimer()
    {
        if(pieceContext.ViewDefinition == null) return;
        
        var view = pieceContext.ViewDefinition.AddView(ViewType.BoardTimer);

        view.Priority = -1;
        view.Ofset = TimerOffset;
        view.SetOfset();
        view.Change(true);
    }
    
    private void OnHideTimer()
    {
        if(pieceContext.ViewDefinition == null) return;
        
        var view = pieceContext.ViewDefinition.AddView(ViewType.BoardTimer);
        var isShow = !IsFilled;
        
        if(isShow == false) view.Priority = 10;
        view.Change(isShow);
    }
}