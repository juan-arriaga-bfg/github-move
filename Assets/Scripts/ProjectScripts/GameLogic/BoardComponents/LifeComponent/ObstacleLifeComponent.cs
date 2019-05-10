using System.Collections.Generic;

public class ObstacleLifeComponent : WorkplaceLifeComponent
{
    public override CurrencyPair Energy => GameDataService.Current.ObstaclesManager.GetPriceByStep(Context.PieceType, current);
    public override string AnalyticsLocation => "skip_obstacle";

    public override string Message
    {
        get
        {
            var message = LocalizationService.Get("gameboard.bubble.message.obstacle", "gameboard.bubble.message.obstacle\n{0}?");

            return GetDelay(current) > WorkerCurrencyLogicComponent.MinDelay
                ? string.Format(message, DateTimeExtension.GetDelayText(GameDataService.Current.ObstaclesManager.GetDelayByStep(Context.PieceType, current), true))
                : message.Replace("\n{0}", "");
        }
    }

    public override string Price
    {
        get
        {
            var key = Context.Draggable == null ? "gameboard.bubble.button.obstacle" : "gameboard.bubble.button.deadfield";
            
            return string.Format(LocalizationService.Get(key, $"{key} {0}"), Energy.ToStringIcon());
        }
    }

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

        if (item == null) return null;
		
        current = item.Step;
        Context.Context.WorkerLogic.Init(Context.CachedPosition, TimerMain);
        
        TimerMain.Delay =  GetDelay(current - 1);
        TimerMain.IsCanceled = TimerMain.Delay > WorkerCurrencyLogicComponent.MinDelay;
        
        if (item.IsStartTimer) TimerMain.Start(item.StartTimeTimer);
        else
        {
            OnTimerStart();
            if (Rewards.IsComplete == false) Locker.Unlock(this);
        }
        
        return item;
    }

    public override void OnAddToBoard(BoardPosition position, Piece context = null)
    {
        base.OnAddToBoard(position, context);
        
        Context.AddView(ViewType.ObstacleProgress);
        
        LocalNotificationsService.Current.RegisterNotifier(new Notifier(TimerMain, NotifyType.RemoveObstacleComplete));
    }

    public override void OnRemoveFromBoard(BoardPosition position, Piece context = null)
    {
        base.OnRemoveFromBoard(position, context);
        
        LocalNotificationsService.Current.UnRegisterNotifier(TimerMain);
    }

    protected override Dictionary<int, int> GetRewards()
    {
        return IsDead
            ? GameDataService.Current.ObstaclesManager.GetPiecesByLastStep(Context.PieceType, current - 1)
            : GameDataService.Current.ObstaclesManager.GetPiecesByStep(Context.PieceType, current - 1);
    }
    
    protected override void Success()
    {
        TimerMain.Delay = GetDelay(current);
        TimerMain.IsCanceled = TimerMain.Delay > WorkerCurrencyLogicComponent.MinDelay;
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

    private int GetDelay(int step)
    {
        return HP == step + 1 ? GameDataService.Current.ObstaclesManager.GetDelayByStep(Context.PieceType, step) : WorkerCurrencyLogicComponent.MinDelay;
    }
}