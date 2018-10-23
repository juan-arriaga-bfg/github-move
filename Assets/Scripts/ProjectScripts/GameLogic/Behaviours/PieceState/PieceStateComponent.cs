using System;
using System.Collections.Generic;

public enum BuildingState
{
    Default,
    Waiting,
    Warning,
    InProgress,
    Complete
}

public class PieceStateComponent : ECSEntity, IPieceBoardObserver
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    public BuildingState StartState = BuildingState.Waiting;
    
    public TimerComponent Timer;
    public Action OnChangeState;
    
    private Piece thisContext;
    
    private BuildingState state;
    public BuildingState State
    {
        get { return state; }
        set
        {
            state = value;
            
            OnChangeState?.Invoke();

            switch (state)
            {
                case BuildingState.Waiting:
                    OnWaiting();
                    break;
                case BuildingState.InProgress:
                    OnStart();
                    break;
            }
            
            if (state == BuildingState.Complete)
            {
                thisContext.Matchable?.Locker.Unlock(this);
                
                var action = thisContext.Context.BoardLogic.MatchActionBuilder.GetMatchAction(new List<BoardPosition>{thisContext.CachedPosition}, thisContext.PieceType, thisContext.CachedPosition);
        
                if(action == null) return;
        
                thisContext.Context.ActionExecutor.AddAction(action);
                return;
            }
            
            thisContext.Matchable?.Locker.Lock(this, false);
        }
    }

    public override void OnRegisterEntity(ECSEntity entity)
    {
        thisContext = entity as Piece;
        
        var def = GameDataService.Current.PiecesManager.GetPieceDef(thisContext.PieceType + 1);
        
        Timer = new TimerComponent{Delay = def.MatchConditionsDef.Delay, Price = def.MatchConditionsDef.FastPrice};
        Timer.OnComplete += OnComplete;
        RegisterComponent(Timer);
    }
    
    public void OnAddToBoard(BoardPosition position, Piece context = null)
    {
        var save = ProfileService.Current.GetComponent<FieldDefComponent>(FieldDefComponent.ComponentGuid);
        var item = save?.GetBuildingSave(position);

        if (item == null)
        {
            State = StartState;
            return;
        }
        
        if (item.State == BuildingState.InProgress)
        {
            Timer.Start(item.StartTime);
            thisContext.Context.WorkerLogic.Init(position, Timer);
        }
        
        State = item.State;
    }

    public void OnMovedFromToStart(BoardPosition @from, BoardPosition to, Piece context = null)
    {
    }

    public void OnMovedFromToFinish(BoardPosition from, BoardPosition to, Piece context = null)
    {
        if (thisContext.Multicellular == null)
        {
            thisContext.Context.WorkerLogic.Replace(from, to);
            return;
        }
        
        foreach (var point in thisContext.Multicellular.Mask)
        {
            var posFrom = thisContext.Multicellular.GetPointInMask(from, point);
            var posTo = thisContext.Multicellular.GetPointInMask(to, point);
            
            if(thisContext.Context.WorkerLogic.Replace(posFrom, posTo) == false) continue;
            
            return;
        }
    }

    public void OnRemoveFromBoard(BoardPosition position, Piece context = null)
    {
    }

    private void OnWaiting()
    {
        thisContext.Context.BoardEvents.RaiseEvent(GameEventsCodes.ClosePieceUI, thisContext.CachedPosition);
        
        var view = thisContext.ViewDefinition.AddView(ViewType.Bubble) as BubbleView;
        
        view.OnHide = () => { State = BuildingState.Warning; };
        view.SetData($"Build Piece:\n{DateTimeExtension.GetDelayText(Timer.Delay)}?", $"Send <sprite name={Currency.Worker.Name}>", piece => Work(), true, false);
        view.Change(true);
    }

    public void OnChange()
    {
        switch (state)
        {
            case BuildingState.Waiting:
                thisContext.ViewDefinition.AddView(ViewType.Bubble).Change(false);
                break;
            case BuildingState.Warning:
                State = BuildingState.Waiting;
                break;
        }
    }

    public void Work(bool isExtra = false)
    {
        var view = thisContext.ViewDefinition.AddView(ViewType.Bubble);
        
        view.Change(false);
        
        if(isExtra == false && thisContext.Context.WorkerLogic.Get(thisContext.CachedPosition, Timer) == false) return;
        
        view.OnHide = null;
        State = BuildingState.InProgress;
    }

    private void OnStart()
    {
        var view = thisContext.ViewDefinition.AddView(ViewType.BoardTimer) as BoardTimerView;
        
        view.SetTimer(Timer);
        view.Change(true);
        view.SetHourglass(true);
        
        if (Timer.IsStarted == false) Timer.Start();
    }
    
    private void OnComplete()
    {
        thisContext.ViewDefinition.AddView(ViewType.BoardTimer).Change(false);
        State = BuildingState.Complete;
        
        if (thisContext.Multicellular == null)
        {
            thisContext.Context.WorkerLogic.Return(thisContext.CachedPosition);
            return;
        }

        foreach (var point in thisContext.Multicellular.Mask)
        {
            var position = thisContext.Multicellular.GetPointInMask(thisContext.CachedPosition, point);
            
            if(thisContext.Context.WorkerLogic.Return(position) == false) continue;
            
            return;
        }
    }
}