﻿using DG.Tweening;
using UnityEngine;

public class ObstacleLifeComponent : StorageLifeComponent
{
    public override CurrencyPair Energy => GameDataService.Current.ObstaclesManager.GetPriceByStep(thisContext.PieceType, current);

    public override string Message => $"Tree chopping:\n{DateTimeExtension.GetDelayText(GameDataService.Current.ObstaclesManager.GetDelayByStep(thisContext.PieceType, current))}";
    public override string Price => $"Chop {Energy.ToStringIcon()}";

    public override void OnAddToBoard(BoardPosition position, Piece context = null)
    {
        HP = thisContext.Context.BoardLogic.MatchDefinition.GetIndexInChain(thisContext.PieceType);
        
        base.OnAddToBoard(position, context);
        
        var timer = thisContext.GetComponent<TimerComponent>(TimerComponent.ComponentGuid);
        
        timer.Price = GameDataService.Current.ObstaclesManager.GetFastPriceByStep(thisContext.PieceType, current - 1);
        timer.Delay = GameDataService.Current.ObstaclesManager.GetDelayByStep(thisContext.PieceType, current - 1);
        
        storage.Capacity = storage.Amount = 1;
    }

    protected override void Success()
    {
        storage.Timer.Price = GameDataService.Current.ObstaclesManager.GetFastPriceByStep(thisContext.PieceType, current);
        storage.Timer.Delay = GameDataService.Current.ObstaclesManager.GetDelayByStep(thisContext.PieceType, current);
    }

    protected override int GetTimerDelay()
    {
        return GameDataService.Current.ObstaclesManager.GetDelayByStep(thisContext.PieceType, current);
    }

    protected override void OnStep()
    {
        var pieces = GameDataService.Current.ObstaclesManager.GetPiecesByStep(thisContext.PieceType, current);
        
        foreach (var key in pieces.Keys)
        {
            storage.SpawnPiece = key;
            break;
        }
        
        storage.SpawnAction = new EjectionPieceAction
        {
            From = thisContext.CachedPosition,
            Pieces = pieces,
            OnComplete = () =>
            {
                OnSpawnRewards();
                thisContext.Context.HintCooldown.Step(HintType.Obstacle);
            }
        };
    }

    protected override void OnComplete()
    {
        storage.SpawnPiece = PieceType.Chest1.Id;
        
        storage.OnScatter = () =>
        {
            storage.OnScatter = null;
            OnSpawnRewards();
        };
    }

    protected override void OnSpawnRewards()
    {
        var rewards = GameDataService.Current.ObstaclesManager.GetRewardByStep(thisContext.PieceType, current);
        
        var sequence = DOTween.Sequence();
        
        for (var i = 0; i < rewards.Count; i++)
        {
            var reward = rewards[i];
            sequence.InsertCallback(0.5f * i, () => AddResourceView.Show(StartPosition(), reward));
        }
                
        thisContext.Context.HintCooldown.Step(HintType.Obstacle);
    }
}