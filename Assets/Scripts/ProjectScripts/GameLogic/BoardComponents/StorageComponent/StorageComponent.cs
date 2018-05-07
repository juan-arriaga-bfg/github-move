﻿using UnityEngine;

public class StorageComponent : IECSComponent, ITimerComponent, IPieceBoardObserver
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    
    public int Guid { get { return ComponentGuid; } }

    public int SpawnPiece;
    
    public int Amount;
    public int Capacity;
    public int Filling;

    public bool ShowTimer;
    
    private ViewDefinitionComponent viewDef;
    
    public TimerComponent Timer { get; private set; }

    private Piece pieceContext;
    
    public void OnRegisterEntity(ECSEntity entity)
    {
        pieceContext = entity as Piece;
        
        Timer = pieceContext.GetComponent<TimerComponent>(TimerComponent.ComponentGuid);
        viewDef = pieceContext.GetComponent<ViewDefinitionComponent>(ViewDefinitionComponent.ComponentGuid);
    }
    
    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }
    
    public void OnAddToBoard(BoardPosition position, Piece context = null)
    {
        if(Timer == null) return;
        
        Timer.OnComplete += Update;

        if (ShowTimer)
        {
            Timer.OnStart += OnShowTimer;
            Timer.OnComplete += OnHideTimer;
        }
        
        if(Filling != Capacity) Timer.Start();
        
        UpdateView();
    }

    public void OnMovedFromTo(BoardPosition @from, BoardPosition to, Piece context = null)
    {
    }

    public void OnRemoveFromBoard(BoardPosition position, Piece context = null)
    {
        if(Timer == null) return;
        
        Timer.Stop();
        
        Timer.OnComplete -= Update;
        
        if(ShowTimer == false) return;

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

    public bool Scatter(out int amount)
    {
        amount = Filling;
        
        if (Filling == 0) return false;
        
        Filling = 0;
        Timer.Start();
        UpdateView();
        
        return true;
    }

    private void OnShowTimer()
    {
        if(viewDef == null) return;
        
        var view = viewDef.AddView(ViewType.BoardTimer);
        
        view.Change(true);
    }
    
    private void OnHideTimer()
    {
        if(viewDef == null) return;
        
        var view = viewDef.AddView(ViewType.BoardTimer);
        
        view.Change(Filling < Capacity);
    }
}