﻿using System;
using UnityEngine;

public class StorageComponent : IECSComponent, ITimerComponent, IPieceBoardObserver
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    
    public int Guid { get { return ComponentGuid; } }

    public int SpawnPiece;
    
    public int Amount;
    public int Capacity;
    public int Filling;

    public bool IsTimerShow;
    public bool IsAutoStart = true;
    
    private ViewDefinitionComponent viewDef;
    
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
        viewDef = pieceContext.GetComponent<ViewDefinitionComponent>(ViewDefinitionComponent.ComponentGuid);
        
        if(Timer == null) return;
        
        Timer.OnComplete += Update;

        if (IsTimerShow)
        {
            Timer.OnStart += OnShowTimer;
            Timer.OnComplete += OnHideTimer;
        }

        if (InitInSave(position) == false && IsAutoStart && Filling != Capacity)
        {
            Timer.Start();
        }
        
        UpdateView();
    }

    private bool InitInSave(BoardPosition position)
    {
        StorageSaveItem item;
        var save = ProfileService.Current.GetComponent<FieldDefComponent>(FieldDefComponent.ComponentGuid);
        
        if (save == null || save.StorageSave == null || save.StorageSave.TryGetValue(position, out item) == false) return false;
        
        long now;
        var steps = Timer.CountOfStepsPassedWhenAppWasInBackground(item.StartTime, out now);
        
        Filling = Mathf.Min(item.Filling + Mathf.Max(steps, 0), Capacity);
        item.StartTime = now;
        
        if (Filling != Capacity)
        {
            Timer.Start(item.StartTime);
        }

        save.StorageSave.Remove(position);

        return true;
    }

    public void OnMovedFromTo(BoardPosition @from, BoardPosition to, Piece context = null)
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
        
        if(Filling < Capacity) Timer.Start();

        UpdateView();
    }
    
    private void UpdateView()
    {
        if(viewDef == null) return;
        
        var view = viewDef.AddView(ViewType.StorageState);
        var isShow = Filling / (float) Capacity > 0.2f;
        
        view.Change(isShow);
        
        if(isShow) pieceContext.Context.BoardEvents.RaiseEvent(GameEventsCodes.ClosePieceMenu, this);
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
        if(viewDef == null) return;
        
        var view = viewDef.AddView(ViewType.BoardTimer);

        view.Priority = -1;
        view.Ofset = TimerOffset;
        view.SetOfset();
        view.Change(true);
    }
    
    private void OnHideTimer()
    {
        if(viewDef == null) return;
        
        var view = viewDef.AddView(ViewType.BoardTimer);
        var isShow = Filling < Capacity;
        
        if(isShow == false) view.Priority = 10;
        view.Change(isShow);
    }
}