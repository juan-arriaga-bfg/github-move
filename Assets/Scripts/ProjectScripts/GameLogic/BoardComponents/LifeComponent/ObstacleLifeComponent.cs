using System;
using System.Collections.Generic;

public class ObstacleLifeComponent : WorkplaceLifeComponent
{
    public override CurrencyPair Energy => GameDataService.Current.ObstaclesManager.GetPriceByStep(Context.PieceType, current);

    public override string Message => string.Format(LocalizationService.Get("gameboard.bubble.message.obstacle", "gameboard.bubble.message.obstacle\n{0}?"), DateTimeExtension.GetDelayText(GameDataService.Current.ObstaclesManager.GetDelayByStep(Context.PieceType, current)));
    public override string Price => string.Format(LocalizationService.Get("gameboard.bubble.button.chop", "gameboard.bubble.button.chop {0}"), Energy.ToStringIcon());
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        base.OnRegisterEntity(entity);
        
        HP = Context.Context.BoardLogic.MatchDefinition.GetIndexInChain(Context.PieceType);
    }
    
    protected override Dictionary<int, int> GetRewards()
    {
        return GameDataService.Current.ObstaclesManager.GetPiecesByStep(Context.PieceType, current - 1);
    }
    
    protected override void Success()
    {
        Timer.Delay = GameDataService.Current.ObstaclesManager.GetDelayByStep(Context.PieceType, current);
    }
    
    protected override void OnStep()
    {
        storage.SpawnAction = new EjectionPieceAction
        {
            GetFrom = () => Context.CachedPosition,
            Pieces = Reward,
            OnComplete = OnSpawnRewards
        };
    }
    
    protected override void OnComplete()
    {
        storage.SpawnPiece = GameDataService.Current.ObstaclesManager.GetReward(Context.PieceType);

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
        var rewards = GameDataService.Current.ObstaclesManager.GetRewardByStep(Context.PieceType, current - 1);
        
        AddResourceView.Show(StartPosition(), rewards);
        Context.Context.HintCooldown.Step(HintType.Obstacle);

        base.OnSpawnRewards();
        
        if (IsDead)
        {
            BoardService.Current.FirstBoard.BoardEvents.RaiseEvent(GameEventsCodes.ObstacleKilled, this);
        }
    }
}