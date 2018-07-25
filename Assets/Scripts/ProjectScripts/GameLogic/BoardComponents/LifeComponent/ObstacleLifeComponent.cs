using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ObstacleLifeComponent : StorageLifeComponent
{
    public override CurrencyPair Energy
    {
        get
        {
            return GameDataService.Current.ObstaclesManager.GetPriceByStep(thisContext.PieceType, current);
        }
    }
    
    public float GetProgressNext
    {
        get { return 1 - (current+1)/(float)HP; }
    }
    
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
        
        var rewards = GameDataService.Current.ObstaclesManager.GetRewardByStep(thisContext.PieceType, current);
        
        var sequence = DOTween.Sequence();
        
        for (var i = 0; i < rewards.Count; i++)
        {
            var reward = rewards[i];
            sequence.InsertCallback(0.5f * (i + 1), () => AddResourceView.Show(StartPosition(), reward));
        }
    }
    
    protected override void OnStep()
    {
        var reward = GameDataService.Current.ObstaclesManager.GetPiecesByStep(thisContext.PieceType, current);
        
        foreach (var key in reward.Keys)
        {
            storage.SpawnPiece = key;
            break;
        }
        
        storage.SpawnAction = new EjectionPieceAction
        {
            From = thisContext.CachedPosition,
            Pieces = reward,
            OnComplete = () =>
            {
                var hint = thisContext.Context.GetComponent<HintCooldownComponent>(HintCooldownComponent.ComponentGuid);

                if (hint != null) hint.Step(HintType.Obstacle);
            }
        };
    }

    protected override void OnComplete()
    {
        var position = thisContext.CachedPosition;
        
        storage.SpawnPiece = PieceType.Chest1.Id;
        
        storage.SpawnAction = new CollapsePieceToAction
        {
            To = position,
            Positions = new List<BoardPosition> {position},
            OnCompleteAction = new SpawnPieceAtAction()
            {
                IsCheckMatch = false,
                At = thisContext.CachedPosition,
                PieceTypeId = GameDataService.Current.ObstaclesManager.GetReward(thisContext.PieceType)
            }
        };
    }
}