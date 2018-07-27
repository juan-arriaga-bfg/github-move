﻿using System.Collections.Generic;
using DG.Tweening;

public class MineLifeComponent : StorageLifeComponent
{
    private MineDef def;
    
    public override CurrencyPair Energy
    {
        get
        {
            return def.Price;
        }
    }
    
    public override string Key
    {
        get { return def.Position.ToSaveString(); }
    }

    public override void OnAddToBoard(BoardPosition position, Piece context = null)
    {
        base.OnAddToBoard(position, context);
        
        var key = new BoardPosition(position.X, position.Y);
        def = GameDataService.Current.MinesManager.GetDef(key);
        
        if(def == null) return;
        
        var timer = thisContext.GetComponent<TimerComponent>(TimerComponent.ComponentGuid);
        
        timer.Delay = def.Delay;
        timer.Price = def.FastPrice;
        
        storage.SpawnPiece = PieceType.Parse(def.Reward.Currency);
        storage.Capacity = storage.Amount = def.Reward.Amount;
        
        HP = def.Size;
    }

    protected override void Success()
    {
        base.Success();
        
        var sequence = DOTween.Sequence();
        sequence.InsertCallback(0.5f, () => AddResourceView.Show(StartPosition(), def.StepReward));
    }

    protected override void OnComplete()
    {
        var position = thisContext.CachedPosition;
                
        storage.OnScatter = () =>
        {
            storage.OnScatter = null;
            thisContext.Context.ActionExecutor.AddAction(new CollapsePieceToAction
            {
                To = position,
                Positions = new List<BoardPosition> {position},
                OnComplete = OnRemove
            });
        };
    }

    private void OnRemove()
    {
        var multi = thisContext.GetComponent<MulticellularPieceBoardObserver>(MulticellularPieceBoardObserver.ComponentGuid);
        
        if(multi == null) return;

        var mask = multi.Mask;
        
        foreach (var maskPoint in mask)
        {
            var point = multi.GetPointInMask(thisContext.CachedPosition, maskPoint);
            
            thisContext.Context.ActionExecutor.AddAction(new SpawnPieceAtAction()
            {
                IsCheckMatch = false,
                At = point,
                PieceTypeId = PieceType.OX1.Id
            });
        }
    }
}