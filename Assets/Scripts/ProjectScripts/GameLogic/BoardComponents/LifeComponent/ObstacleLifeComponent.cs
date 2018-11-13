﻿using System;

public class ObstacleLifeComponent : StorageLifeComponent
{
    public override CurrencyPair Energy => GameDataService.Current.ObstaclesManager.GetPriceByStep(thisContext.PieceType, current);

    public override string Message => $"Tree chopping:\n{DateTimeExtension.GetDelayText(GameDataService.Current.ObstaclesManager.GetDelayByStep(thisContext.PieceType, current))}";
    public override string Price => $"Chop {Energy.ToStringIcon()}";

    private int spawnPiece = PieceType.None.Id;
    public override int StorageSpawnPiece => spawnPiece;

    public override void OnRegisterEntity(ECSEntity entity)
    {
        base.OnRegisterEntity(entity);
        
        HP = thisContext.Context.BoardLogic.MatchDefinition.GetIndexInChain(thisContext.PieceType);
    }

    protected override void InitStorage()
    {
        storage.Capacity = storage.Amount = 1;
    }

    protected override Action InitInSaveStorage(LifeSaveItem item)
    {
        storage.Timer.Delay = GameDataService.Current.ObstaclesManager.GetDelayByStep(thisContext.PieceType, current - 1);
        
        return base.InitInSaveStorage(item);
    }

    protected override void InitInSaveReward(LifeSaveItem item)
    {
        base.InitInSaveReward(item);
        
        if(Reward == null || !storage.IsFilled || IsDead == false) return;
        
        int value;

        spawnPiece = item.StorageSpawnPiece;

        if (Reward.TryGetValue(spawnPiece, out value)) Reward[spawnPiece] = value + 1;
        else Reward.Add(spawnPiece, 1);
    }

    protected override void Success()
    {
        storage.Timer.Delay = GameDataService.Current.ObstaclesManager.GetDelayByStep(thisContext.PieceType, current);
    }
    
    protected override void OnStep()
    {
        OnStep(false);
    }
    
    private void OnStep(bool isRemoveMain)
    {
        if(Reward == null || Reward.Count == 0) Reward = GameDataService.Current.ObstaclesManager.GetPiecesByStep(thisContext.PieceType, current - 1);
        
        foreach (var key in Reward.Keys)
        {
            storage.SpawnPiece = key;
            break;
        }

        if (isRemoveMain)
        {
            if (spawnPiece == PieceType.None.Id) spawnPiece = storage.SpawnPiece;
            else storage.SpawnPiece = spawnPiece;
            
            var value = Reward[storage.SpawnPiece];
            
            value--;
            
            if (value == 0) Reward.Remove(storage.SpawnPiece);
            else Reward[storage.SpawnPiece] = value;
        }
        
        storage.SpawnAction = new EjectionPieceAction
        {
            GetFrom = () => thisContext.CachedPosition,
            Pieces = Reward,
            OnComplete = () =>
            {
                spawnPiece = PieceType.None.Id;
                Reward = null;
                OnSpawnRewards();
            }
        };
    }
    
    protected override void OnComplete()
    {
        storage.SpawnPiece = GameDataService.Current.ObstaclesManager.GetReward(thisContext.PieceType);

        if (storage.SpawnPiece == PieceType.None.Id)
        {
            OnStep(true);
            return;
        }
        
        storage.OnScatter = () =>
        {
            storage.OnScatter = null;
            OnSpawnRewards();
        };
    }

    protected override void OnSpawnRewards()
    {
        var rewards = GameDataService.Current.ObstaclesManager.GetRewardByStep(thisContext.PieceType, current - 1);
        
        AddResourceView.Show(StartPosition(), rewards);
        thisContext.Context.HintCooldown.Step(HintType.Obstacle);

        base.OnSpawnRewards();
        
        if (IsDead)
        {
            BoardService.Current.FirstBoard.BoardEvents.RaiseEvent(GameEventsCodes.ObstacleKilled, this);
        }
    }
}