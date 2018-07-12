using System;
using System.Collections.Generic;
using UnityEngine;

public class MineLifeComponent : LifeComponent, IPieceBoardObserver
{
    private StorageComponent storage;
    private MineDef def;
    
    public CurrencyPair Energy
    {
        get
        {
            return def.Conditions.Find(pair => pair.Currency == Currency.Energy.Name);
        }
    }
    
    public CurrencyPair Worker
    {
        get
        {
            return  def.Conditions.Find(pair => pair.Currency == Currency.Worker.Name);
        }
    }
    
    public string Key
    {
        get { return string.Format("{0}_{1}", thisContext.PieceType, def.Position); }
    }
    
    public void OnAddToBoard(BoardPosition position, Piece context = null)
    {
        var key = new BoardPosition(position.X, position.Y);
        def = GameDataService.Current.MinesManager.GetDef(key);
        
        if(def == null) return;
        
        var timer = thisContext.GetComponent<TimerComponent>(TimerComponent.ComponentGuid);
        
        timer.Delay = def.Delay;
        
        if(storage == null) storage = thisContext.GetComponent<StorageComponent>(StorageComponent.ComponentGuid);
        
        storage.SpawnPiece = PieceType.Parse(def.Reward.Currency);
        storage.Capacity = storage.Amount = def.Reward.Amount;
        
        HP = def.Size;
        current = 0;//GameDataService.Current.ObstaclesManager.GetSaveStep(position);
    }

    public void OnMovedFromTo(BoardPosition from, BoardPosition to, Piece context = null)
    {
    }

    public void OnRemoveFromBoard(BoardPosition position, Piece context = null)
    {
        var multi = thisContext.GetComponent<MulticellularPieceBoardObserver>(MulticellularPieceBoardObserver.ComponentGuid);
        
        if(multi == null) return;

        var mask = multi.Mask;
        
        foreach (var maskPoint in mask)
        {
            var point = multi.GetPointInMask(position, maskPoint);
            
            thisContext.Context.ActionExecutor.AddAction(new SpawnPieceAtAction()
            {
                IsCheckMatch = false,
                At = point,
                PieceTypeId = PieceType.OX1.Id
            });
        }
    }

    public bool Damage(Action onComplete)
    {
        if (current == HP) return false;

        var isSuccess = false;

        if (CurrencyHellper.IsCanPurchase(def.Conditions) == false) return false;

        thisContext.Context.WorkerLogic.Get(Key, storage.Timer.Delay);
        
        CurrencyHellper.Purchase(Currency.Damage.Name, 1, Energy, success =>
        {
            if(success == false) return;
            
            isSuccess = true;
            
            var position = thisContext.CachedPosition;
            
            Damage(Worker == null ? 1 : Worker.Amount);
            
            storage.Timer.Start();
            
            if (current == HP)
            {
                storage.OnScatter = () =>
                {
                    storage.OnScatter = null;
                    thisContext.Context.ActionExecutor.AddAction(new CollapsePieceToAction
                    {
                        To = position,
                        Positions = new List<BoardPosition> {position}
                    });
                };
            }
        });
        
        return isSuccess;
    }
}