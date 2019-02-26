using System.Collections.Generic;

public class MineLifeComponent : WorkplaceLifeComponent
{
    private PieceMineDef def;
    
    public override CurrencyPair Energy => def.Price;
    public override string AnalyticsLocation => $"skip_mine{(TimerCooldown.IsExecuteable() ? "_cooldown" : "")}";
    public override string Message => string.Format(LocalizationService.Get("gameboard.bubble.message.mine", "gameboard.bubble.message.mine\n{0}?"), DateTimeExtension.GetDelayText(def.Delay));
    public override string Price => TimerCooldown.IsExecuteable()
        ? string.Format(LocalizationService.Get("gameboard.bubble.button.wait", "gameboard.bubble.button.wait\n{0}"), TimerCooldown.CompleteTime.GetTimeLeftText())
        : string.Format(LocalizationService.Get("gameboard.bubble.button.clear", "gameboard.bubble.button.clear {0}"), Energy.ToStringIcon());

    public override void OnRegisterEntity(ECSEntity entity)
    {
        base.OnRegisterEntity(entity);
        
        def = GameDataService.Current.PiecesManager.GetPieceDef(Context.PieceType).MineDef;
        
        TimerWork.Delay = def.Delay;
        HP = def.Size;
        
        TimerCooldown = new TimerComponent{Delay = def.Cooldown};
        RegisterComponent(TimerCooldown);
    }

    protected override LifeSaveItem InitInSave(BoardPosition position)
    {
        var item = base.InitInSave(position);

        if (item == null) return null;

        if (item.IsStartCooldown)
        {
            TimerCooldown.Start(item.StartTimeCooldown);
            Locker.Unlock(this);
        }
        else if (item.IsStartTimer == false && Rewards.IsComplete == false) Locker.Unlock(this);

        return item;
    }
    
    public override LifeSaveItem Save()
    {
        var save =  new LifeSaveItem
        {
            Step = current,
            Position = Context.CachedPosition,
            IsStartTimer = TimerWork.IsExecuteable(),
            StartTimeTimer = TimerWork.StartTimeLong,
            IsStartCooldown = TimerCooldown.IsExecuteable(),
            StartTimeCooldown = TimerCooldown.StartTimeLong
        };
        
        return save;
    }
    
    public override bool Damage(bool isExtra = false)
    {
        if (TimerCooldown.IsExecuteable() == false) return base.Damage(isExtra);
        
        UIMessageWindowController.CreateTimerCompleteMessage(
            LocalizationService.Get("window.timerComplete.message.production", "window.timerComplete.message.production"),
            AnalyticsLocation,
            TimerCooldown);
        
        return false;
    }

    protected override void OnComplete()
    {
    }

    protected override Dictionary<int, int> GetRewards()
    {
        var pieces = new Dictionary<int, int> {{PieceType.Parse(def.Reward.Currency), def.Reward.Amount}};
        
        if (IsDead == false) return pieces;
        
        foreach (var pair in def.LastRewards)
        {
            pieces.Add(PieceType.Parse(pair.Currency), pair.Amount);
        }
        
        return pieces;
    }

    protected override void OnSpawnCurrencyRewards(bool isComplete)
    {
        if (isComplete)
        {
            AddResourceView.Show(StartPosition(), def.StepRewards);
            
            if (IsDead)
            {
                TimerCooldown.Start();
                current = 0;
            }
        }
        
        base.OnSpawnCurrencyRewards(isComplete);
    }
    
    protected override void OnTimerComplete()
    {
        base.OnTimerComplete();
        
        GameDataService.Current.QuestsManager.StartNewQuestsIfAny(); // To ensure that QuestStartConditionMineUsedComponent triggered
        BoardService.Current.FirstBoard.BoardEvents.RaiseEvent(GameEventsCodes.MineUsed, this);
    }
}