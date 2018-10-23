public class ObstacleLifeComponent : StorageLifeComponent
{
    public override CurrencyPair Energy => GameDataService.Current.ObstaclesManager.GetPriceByStep(thisContext.PieceType, current);

    public override string Message => $"Tree chopping:\n{DateTimeExtension.GetDelayText(GameDataService.Current.ObstaclesManager.GetDelayByStep(thisContext.PieceType, current))}";
    public override string Price => $"Chop {Energy.ToStringIcon()}";

    public override void OnRegisterEntity(ECSEntity entity)
    {
        base.OnRegisterEntity(entity);
        
        HP = thisContext.Context.BoardLogic.MatchDefinition.GetIndexInChain(thisContext.PieceType);
    }

    protected override void InitStorage()
    {
        storage.Capacity = storage.Amount = 1;
    }

    protected override void InitInSaveStorage()
    {
        storage.Timer.Price = GameDataService.Current.ObstaclesManager.GetFastPriceByStep(thisContext.PieceType, current - 1);
        storage.Timer.Delay = GameDataService.Current.ObstaclesManager.GetDelayByStep(thisContext.PieceType, current - 1);
        
        base.InitInSaveStorage();
    }

    protected override void Success()
    {
        storage.Timer.Price = GameDataService.Current.ObstaclesManager.GetFastPriceByStep(thisContext.PieceType, current);
        storage.Timer.Delay = GameDataService.Current.ObstaclesManager.GetDelayByStep(thisContext.PieceType, current);
    }
    
    protected override void OnStep()
    {
        OnStep(false);
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

    private void OnStep(bool isRemoveMain)
    {
        var pieces = GameDataService.Current.ObstaclesManager.GetPiecesByStep(thisContext.PieceType, current);
        
        foreach (var key in pieces.Keys)
        {
            storage.SpawnPiece = key;
            break;
        }

        if (isRemoveMain)
        {
            var value = pieces[storage.SpawnPiece];
            
            value--;
            
            if (value == 0) pieces.Remove(storage.SpawnPiece);
            else pieces[storage.SpawnPiece] = value;
        }
        
        storage.SpawnAction = new EjectionPieceAction
        {
            From = thisContext.CachedPosition,
            Pieces = pieces,
            OnComplete = OnSpawnRewards
        };
    }

    protected override void OnSpawnRewards()
    {
        var rewards = GameDataService.Current.ObstaclesManager.GetRewardByStep(thisContext.PieceType, current);
        
        AddResourceView.Show(StartPosition(), rewards);
        thisContext.Context.HintCooldown.Step(HintType.Obstacle);

        base.OnSpawnRewards();
    }
    
    protected override void OnTimerComplete()
    {
        base.OnTimerComplete();

        if (IsDead)
        {
            BoardService.Current.FirstBoard.BoardEvents.RaiseEvent(GameEventsCodes.ObstacleKilled, this);
        }
    }
}