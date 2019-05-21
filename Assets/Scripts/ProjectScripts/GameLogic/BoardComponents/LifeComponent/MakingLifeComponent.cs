using System.Collections.Generic;

public class MakingLifeComponent : WorkplaceLifeComponent
{
    private PieceMakingDef def;
    
    public override CurrencyPair Energy => def.Price;
    public override string AnalyticsLocation => $"skip_monument";
    public override string Message => string.Format(LocalizationService.Get("gameboard.bubble.message.castle.make", "gameboard.bubble.message.castle.make {0}"), Energy.ToStringIcon());
    public override string Price => TimerCooldown.IsExecuteable() ? string.Format(LocalizationService.Get("gameboard.bubble.button.wait", "gameboard.bubble.button.wait\n{0}"), TimerMain.CompleteTime.GetTimeLeftText()) : base.Price;

    public override TimerComponent TimerMain => TimerCooldown;
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        base.OnRegisterEntity(entity);
        
        def = GameDataService.Current.PiecesManager.GetPieceDef(Context.PieceType).MakingDef;
        
        HP = -1;
        TimerWork.Delay = WorkerCurrencyLogicComponent.MinDelay;
        
        TimerCooldown = new TimerComponent{Delay = def.Delay};
        RegisterComponent(TimerCooldown);
    }

    private void OnComplete()
    {
        NSAudioService.Current.Play(SoundId.CastleMagic);
    }
    
    public override void OnAddToBoard(BoardPosition position, Piece context = null)
    {
        base.OnAddToBoard(position, context);
        TimerMain.OnComplete += OnComplete;

        if (Context.Multicellular != null)
        {
            LocalNotificationsService.Current.RegisterNotifier(new Notifier(TimerMain, NotifyType.MonumentRefreshComplete));   
        }
    }

    public override void OnRemoveFromBoard(BoardPosition position, Piece context = null)
    {
        base.OnRemoveFromBoard(position, context);
        TimerMain.OnComplete -= OnComplete;
        
        if (Context.Multicellular != null)
        {
            LocalNotificationsService.Current.UnRegisterNotifier(TimerMain);
        }
    }

    protected override Dictionary<int, int> GetRewards()
    {
        return GameDataService.Current.PiecesManager.GetSequence(def.Uid).GetNextDict(def.PieceAmount);
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
        return new LifeSaveItem
        {
            Step = current,
            Position = Context.CachedPosition,
            IsStartTimer = TimerWork.IsExecuteable(),
            StartTimeTimer = TimerWork.StartTimeLong,
            IsStartCooldown = TimerCooldown.IsExecuteable(),
            StartTimeCooldown = TimerCooldown.StartTimeLong
        };
    }
    
    public override bool Damage(bool isExtra = false)
    {
        if (TimerCooldown.IsExecuteable() == false)
        {
            var isDamage = base.Damage(isExtra);
            if (isDamage) (Context.ActorView as MakingBuildingPieceView)?.PlayWorkAnimation();
            return isDamage;
        }
        
        UIMessageWindowController.CreateTimerCompleteMessage(
            LocalizationService.Get("window.timerComplete.message.castle", "window.timerComplete.message.castle"),
            AnalyticsLocation,
            PieceType.Parse(Context.PieceType),
            TimerCooldown);

        return false;
    }
    
    protected override void OnSpawnCurrencyRewards(bool isComplete)
    {
        if (isComplete)
        {
            AddResourceView.Show(StartPosition(), def.StepRewards);
            TimerCooldown.Start();
        }
        
        base.OnSpawnCurrencyRewards(isComplete);
    }
    
    protected override void OnTimerComplete()
    {
        base.OnTimerComplete();
        (Context.ActorView as MakingBuildingPieceView)?.CompleteWorkAnimation();
    }
}