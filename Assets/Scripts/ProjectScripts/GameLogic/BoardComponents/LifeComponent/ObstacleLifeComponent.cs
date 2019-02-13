using System.Collections.Generic;

public class ObstacleLifeComponent : WorkplaceLifeComponent
{
    public override CurrencyPair Energy => GameDataService.Current.ObstaclesManager.GetPriceByStep(Context.PieceType, current);
    public override string AnalyticsLocation => "skip_obstacle";
    public override string Message => string.Format(LocalizationService.Get("gameboard.bubble.message.obstacle", "gameboard.bubble.message.obstacle\n{0}?"), DateTimeExtension.GetDelayText(GameDataService.Current.ObstaclesManager.GetDelayByStep(Context.PieceType, current)));
    public override string Price => string.Format(LocalizationService.Get("gameboard.bubble.button.chop", "gameboard.bubble.button.chop {0}"), Energy.ToStringIcon());
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        base.OnRegisterEntity(entity);
        
        HP = Context.Context.BoardLogic.MatchDefinition.GetIndexInChain(Context.PieceType);
    }

    protected override LifeSaveItem InitInSave(BoardPosition position)
    {
        Rewards.InitInSave(position);
		
        var save = ProfileService.Current.GetComponent<FieldDefComponent>(FieldDefComponent.ComponentGuid);
        var item = save?.GetLifeSave(position);
		
        if(item == null) return null;
		
        current = item.Step;
        Context.Context.WorkerLogic.Init(Context.CachedPosition, TimerMain);
        
        TimerMain.Delay = GameDataService.Current.ObstaclesManager.GetDelayByStep(Context.PieceType, current - 1);
        
        if (item.IsStartTimer) TimerMain.Start(item.StartTimeTimer);
        else
        {
            OnTimerStart();
            Locker.Unlock(this);
        }
        
        return item;
    }

    protected override Dictionary<int, int> GetRewards()
    {
        return IsDead
            ? GameDataService.Current.ObstaclesManager.GetPiecesByLastStep(Context.PieceType, current - 1)
            : GameDataService.Current.ObstaclesManager.GetPiecesByStep(Context.PieceType, current - 1);
    }
    
    protected override void Success()
    {
        TimerMain.Delay = GameDataService.Current.ObstaclesManager.GetDelayByStep(Context.PieceType, current);
    }
    
    protected override void OnSpawnCurrencyRewards(bool isComplete)
    {
        if (isComplete)
        {
            var rewards = GameDataService.Current.ObstaclesManager.GetRewardByStep(Context.PieceType, current - 1);
        
            AddResourceView.Show(StartPosition(), rewards);
            Context.Context.HintCooldown.Step(HintType.Obstacle);
            
            if (IsDead) BoardService.Current.FirstBoard.BoardEvents.RaiseEvent(GameEventsCodes.ObstacleKilled, this);
        }
        
        base.OnSpawnCurrencyRewards(isComplete);
    }
}