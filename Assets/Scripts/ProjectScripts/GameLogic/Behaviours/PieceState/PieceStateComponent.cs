﻿using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
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
                    if (Timer.IsStarted == false) Timer.Start();
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

        Timer = new TimerComponent {Delay = def.Delay, IsCanceled = thisContext.Multicellular == null};
        RegisterComponent(Timer);
    }
    
    public void OnAddToBoard(BoardPosition position, Piece context = null)
    {
        Timer.OnComplete += OnComplete;
        Timer.OnStart += OnStart;

        if (Timer != null && thisContext.Multicellular != null)
        {
            LocalNotificationsService.Current.RegisterNotifier(new Notifier(Timer, NotifyType.MonumentBuild));
        }
        
        var save = ProfileService.Current.GetComponent<FieldDefComponent>(FieldDefComponent.ComponentGuid);
        var item = save?.GetBuildingSave(position);

        if (item == null)
        {
            State = StartState;
            
            if (thisContext.Multicellular == null) return;
            
            foreach (var point in thisContext.Multicellular.Mask)
            {
                var pos = thisContext.Multicellular.GetPointInMask(position, point);
            
                if(thisContext.Context.WorkerLogic.Init(pos, Timer) == false) continue;

                thisContext.Context.WorkerLogic.Replace(pos, position);
                return;
            }
            
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
        if(Timer == null) return;
        
        if (thisContext.Multicellular != null)
        {
            LocalNotificationsService.Current.UnRegisterNotifier(Timer);
        }
        
        Timer.OnComplete -= OnComplete;
        Timer.OnStart -= OnStart;
    }

    private void OnWaiting()
    {
        thisContext.Context.BoardEvents.RaiseEvent(GameEventsCodes.ClosePieceUI, thisContext.CachedPosition);
        
        var view = thisContext.ViewDefinition.AddView(ViewType.Bubble) as BubbleView;
        
        var title = string.Format(LocalizationService.Get("gameboard.bubble.message.piece", "gameboard.bubble.message.piece\n{0}?"), DateTimeExtension.GetDelayText(Timer.Delay, true));
        var button = string.Format(LocalizationService.Get("gameboard.bubble.button.send", "gameboard.bubble.button.send {0}"), $"<sprite name={Currency.Worker.Icon}>");
        
        view.OnHide = () => { State = BuildingState.Warning; };
        view.SetData(title, button, piece => Work(), true, false);
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

        if (isExtra == false && thisContext.Context.WorkerLogic.Get(thisContext.CachedPosition, Timer) == false) return;
        
        view.OnHide = null;
        State = BuildingState.InProgress;

        if (!isExtra) return;
        
        Timer.StartTime = Timer.StartTime.AddSeconds(-Mathf.Min(Timer.Delay, GameDataService.Current.ConstantsManager.ExtraWorkerDelay));
    }

    private void OnStart()
    {
        var view = thisContext.ViewDefinition.AddView(ViewType.BoardTimer) as BoardTimerView;
        
        PlaySoundOnStart();
        
        view.SetTimer(Timer);
        view.Change(true);
        view.SetHourglass(true);
    }

    private void PlaySoundOnStart()
    {
        NSAudioService.Current.Play(SoundId.WorkerBuild);
    }

    private void PlaySoundOnEnd()
    {
        var typeDef = PieceType.GetDefById(thisContext.PieceType);

        if (typeDef.Filter.HasFlag(PieceTypeFilter.Fake))
        {
            NSAudioService.Current.Play(SoundId.BuildMain);
        }
        else
        {
            NSAudioService.Current.Play(SoundId.BuildCastle);
        }
    }
    
    private void OnComplete()
    {
        thisContext.ViewDefinition.AddView(ViewType.BoardTimer).Change(false);
        State = BuildingState.Complete;
        
        PlaySoundOnEnd();
        
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
    
    public void Cancel()
    {
        Timer.Stop();
        thisContext.ViewDefinition.AddView(ViewType.BoardTimer).Change(false);
        State = BuildingState.Warning;
        thisContext.Context.WorkerLogic.Return(thisContext.CachedPosition);
    }
}