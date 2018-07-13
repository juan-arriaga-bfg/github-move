using System.Collections.Generic;
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
    
    public override CurrencyPair Worker
    {
        get
        {
            return new CurrencyPair {Currency = Currency.Worker.Name, Amount = 1};
        }
    }
    
    public override List<CurrencyPair> Conditions
    {
        get { return new List<CurrencyPair> {Energy, Worker}; }
    }
    
    public override string Key
    {
        get
        {
            return string.Format("{0}_{1}", thisContext.PieceType, thisContext.CachedPosition);
        }
    }
    
    public float GetProgressNext
    {
        get { return 1 - (current+1)/(float)HP; }
    }
    
    public override void OnAddToBoard(BoardPosition position, Piece context = null)
    {
        base.OnAddToBoard(position, context);
        
        var timer = thisContext.GetComponent<TimerComponent>(TimerComponent.ComponentGuid);
        
        timer.Delay = GameDataService.Current.ObstaclesManager.GetDelayByStep(thisContext.PieceType, current);
        
        storage.Capacity = storage.Amount = 1;
        
        current = GameDataService.Current.ObstaclesManager.GetSaveStep(position);
        HP = thisContext.Context.BoardLogic.MatchDefinition.GetIndexInChain(thisContext.PieceType);
    }

    protected override void Success()
    {
        storage.Timer.Price = GameDataService.Current.ObstaclesManager.GetFastPriceByStep(thisContext.PieceType, current);
        storage.Timer.Delay = GameDataService.Current.ObstaclesManager.GetDelayByStep(thisContext.PieceType, current);
    }
    
    protected override void OnStep()
    {
        var reward = GameDataService.Current.ObstaclesManager.GetRewardByStep(thisContext.PieceType, current);
        
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