using System;
using System.Collections.Generic;
using UnityEngine;

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
    
    public TimerComponent Timer;
    public Action OnChangeState;
    
    private Piece context;
    private ViewDefinitionComponent viewDef;
    
    private string key => context.CachedPosition.ToSaveString();
    
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
                context.Matchable?.Locker.Unlock(this);
                
                var action = context.Context.BoardLogic.MatchActionBuilder.GetMatchAction(new List<BoardPosition>{context.CachedPosition}, context.PieceType, context.CachedPosition);
        
                if(action == null) return;
        
                context.Context.ActionExecutor.AddAction(action);
                return;
            }
            
            context.Matchable?.Locker.Lock(this, false);
        }
    }

    public override void OnRegisterEntity(ECSEntity entity)
    {
        context = entity as Piece;
        
        var def = GameDataService.Current.PiecesManager.GetPieceDef(context.PieceType + 1);
        
        viewDef = context.GetComponent<ViewDefinitionComponent>(ViewDefinitionComponent.ComponentGuid);
        
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
            State = BuildingState.Waiting;
            return;
        }
        
        if (item.State == BuildingState.InProgress) Timer.Start(item.StartTime);
        
        State = item.State;
    }

    public void OnMovedFromToStart(BoardPosition @from, BoardPosition to, Piece context = null)
    {
    }

    public void OnMovedFromToFinish(BoardPosition @from, BoardPosition to, Piece context = null)
    {
    }

    public void OnRemoveFromBoard(BoardPosition position, Piece context = null)
    {
    }

    private void OnWaiting()
    {
        var view = viewDef.AddView(ViewType.Bubble) as BubbleView;
        
        view.OnHide = () => { State = BuildingState.Warning; };
        view.SetData("Build Piece?", $"Send <sprite name={Currency.Worker.Name}>", piece => OnClick(), true, false);
        view.Change(true);
    }

    public void OnChange()
    {
        switch (state)
        {
            case BuildingState.Waiting:
                viewDef.AddView(ViewType.Bubble).Change(false);
                break;
            case BuildingState.Warning:
                State = BuildingState.Waiting;
                break;
        }
    }

    private void OnClick()
    {
        if(context.Context.WorkerLogic.Get(key, Timer.Delay) == false) return;
        
        var view = viewDef.AddView(ViewType.Bubble);
        
        view.OnHide = null;
        view.Change(false);
        
        State = BuildingState.InProgress;
    }

    private void OnStart()
    {
        var view = viewDef.AddView(ViewType.BoardTimer) as BoardTimerView;
        
        view.SetTimer(Timer);
        view.Change(true);

        if (Timer.IsStarted == false) Timer.Start();
    }
    
    public void Fast()
    {
        Timer.Stop();
        Timer.OnComplete();
        context.Context.WorkerLogic.Return(key);
    }
    
    private void OnComplete()
    {
        viewDef.AddView(ViewType.BoardTimer).Change(false);
        State = BuildingState.Complete;
    }
}