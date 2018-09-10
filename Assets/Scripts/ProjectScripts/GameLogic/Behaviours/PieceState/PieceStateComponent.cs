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

    public BuildingState StartState = BuildingState.Waiting;
    
    public TimerComponent Timer;
    public Action OnChangeState;
    
    private Piece thisContext;
    private ViewDefinitionComponent viewDef;
    
    private string key => thisContext.CachedPosition.ToSaveString();
    
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
        
        viewDef = thisContext.GetComponent<ViewDefinitionComponent>(ViewDefinitionComponent.ComponentGuid);
        
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
        
        if (item.State == BuildingState.InProgress) Timer.Start(item.StartTime);
        
        State = item.State;
    }

    public void OnMovedFromToStart(BoardPosition @from, BoardPosition to, Piece context = null)
    {
    }

    public void OnMovedFromToFinish(BoardPosition from, BoardPosition to, Piece context = null)
    {
        if (thisContext.Multicellular == null)
        {
            thisContext.Context.WorkerLogic.Replase(from.ToSaveString(), to.ToSaveString());
            return;
        }
        
        foreach (var point in thisContext.Multicellular.Mask)
        {
            var posFrom = thisContext.Multicellular.GetPointInMask(from, point);
            var posTo = thisContext.Multicellular.GetPointInMask(to, point);
            
            if(thisContext.Context.WorkerLogic.Replase(posFrom.ToSaveString(), posTo.ToSaveString()) == false) continue;
            
            return;
        }
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
        if(thisContext.Context.WorkerLogic.Get(key, Timer.Delay) == false) return;
        
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
        
        if (thisContext.Multicellular == null)
        {
            thisContext.Context.WorkerLogic.Return(key);
            return;
        }

        foreach (var point in thisContext.Multicellular.Mask)
        {
            var position = thisContext.Multicellular.GetPointInMask(thisContext.CachedPosition, point);
            
            if(thisContext.Context.WorkerLogic.Return(position.ToSaveString()) == false) continue;
            
            return;
        }
    }
    
    private void OnComplete()
    {
        viewDef.AddView(ViewType.BoardTimer).Change(false);
        State = BuildingState.Complete;
    }
}