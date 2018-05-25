using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProductionState
{
    None,
    InProgress,
    Full,
    Waiting,
    Completed
}

public class ProductionComponent : IECSComponent, ITimerComponent, IPieceBoardObserver
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid { get { return ComponentGuid; } }
    
    private TimerComponent timer;
    private ViewDefinitionComponent viewDef;
    private ProductionDef def;

    private Dictionary<int, KeyValuePair<int, int>> storage;
    private int waitingCount;

    private Piece context;
    
    public TimerComponent Timer
    {
        get { return timer; }
    }

    public string Target
    {
        get { return def.Target; }
    }

    public Dictionary<int, KeyValuePair<int, int>> Storage
    {
        get { return storage; }
    }
    
    public ProductionState State { get; set; }
    
    public void OnRegisterEntity(ECSEntity entity)
    {
        context = entity as Piece;

        def = GameDataService.Current.ProductionManager.GetProduction(context.PieceType);
        
        if(def == null) return;
        
        timer = new TimerComponent {Delay = def.Delay, OnStart = OnStart, OnComplete = OnComplete};
        context.RegisterComponent(timer);
        viewDef = context.GetComponent<ViewDefinitionComponent>(ViewDefinitionComponent.ComponentGuid);

        Restart();
    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }

    public void Restart()
    {
        State = ProductionState.InProgress;
        storage = new Dictionary<int, KeyValuePair<int, int>>();

        foreach (var price in def.Prices)
        {
            var key = PieceType.Parse(price.Currency);
            storage.Add(key, new KeyValuePair<int, int>(price.Amount, 0));
        }

        waitingCount = storage.Count;
    }

    public bool Check(int resource)
    {
        KeyValuePair<int, int> value;
        
        var isReacts = storage.TryGetValue(resource, out value) && value.Key != value.Value;

        if (isReacts)
        {
            Change(true);
        }
        
        return isReacts;
    }
    
    public void Hide()
    {
        Change(false);
    }
    
    public void Change()
    {
        var prod = viewDef.AddView(ViewType.Production);
        UIBoardView warning;
        
        switch (State)
        {
            case ProductionState.None:
                prod.Change(false);
                warning = viewDef.AddView(ViewType.ProductionWarning);
                warning.Priority = warning.Layer;
                warning.Change(false);
                break;
            case ProductionState.InProgress:
                prod.Change(!prod.IsShow);
                break;
            case ProductionState.Full:
                prod.Change(!prod.IsShow);
                warning = viewDef.AddView(ViewType.ProductionWarning);
                warning.Priority = warning.Layer;
                warning.Change(!warning.IsShow);
                break;
            case ProductionState.Waiting:
                prod.Change(false);
                warning = viewDef.AddView(ViewType.ProductionWarning);
                warning.Priority = -2;
                warning.Change(true);
                break;
            case ProductionState.Completed:
                prod.Change(false);
                warning = viewDef.AddView(ViewType.ProductionWarning);
                warning.Priority = -2;
                warning.Change(true);
                break;
        }
    }
    
    public void Change(bool isShow, int priority = 0)
    {
        var view = viewDef.AddView(ViewType.Production);

        if (priority != 0)
        {
            view.Priority = priority;
        }
        
        view.Change(isShow);
    }
    
    public bool Add(int resource)
    {
        KeyValuePair<int, int> value;
        
        if (storage.TryGetValue(resource, out value) == false || value.Key == value.Value) return false;

        value = new KeyValuePair<int, int>(value.Key, value.Value + 1);
        storage[resource] = value;
        
        if (value.Key == value.Value) waitingCount--;
        
        if (waitingCount == 0)
        {
            State = ProductionState.Full;
            Change(true, -1);
        }
        
        return true;
    }

    public void Start()
    {
        State = ProductionState.Waiting;
        Change();
        timer.Start();
    }

    public void Fast()
    {
        timer.Stop();
        timer.OnComplete();
    }

    public void Complete()
    {
        State = ProductionState.None;
        Change();
        Restart();

        var piece = PieceType.Parse(def.Target);

        if (piece != PieceType.None.Id)
        {
            context.Context.ActionExecutor.AddAction(new ChestRewardAction
            {
                IsAddCollection = false,
                From = context.CachedPosition,
                Pieces = new Dictionary<int, int>
                {
                    {piece, 1}
                }
            });
            return;
        }
        
        context.Context.ActionExecutor.AddAction(new ChestRewardAction
        {
            IsAddCollection = false,
            From = context.CachedPosition,
            Chargers = new Dictionary<string, int>
            {
                {def.Target, 1}
            }
        });
    }
    
    private void OnStart()
    {
        if(viewDef == null) return;
        
        var view = viewDef.AddView(ViewType.BoardTimer);
        view.Ofset = new Vector3(0f, 2.3f);
        view.SetOfset();
        view.Change(true);
    }
    
    private void OnComplete()
    {
        if(viewDef == null) return;
        
        var view = viewDef.AddView(ViewType.BoardTimer);
        
        view.Change(false);
        
        State = ProductionState.Completed;
        Change();
    }

    public void OnAddToBoard(BoardPosition position, Piece context = null)
    {
        context.Context.ProductionLogic.Add(this);
    }

    public void OnMovedFromTo(BoardPosition @from, BoardPosition to, Piece context = null)
    {
    }

    public void OnRemoveFromBoard(BoardPosition position, Piece context = null)
    {
        timer.Stop();
        context.Context.ProductionLogic.Remove(this);
    }
}