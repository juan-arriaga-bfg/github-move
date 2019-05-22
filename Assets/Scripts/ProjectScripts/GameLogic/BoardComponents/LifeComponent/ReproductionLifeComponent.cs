using System.Collections.Generic;

public class ReproductionLifeComponent : WorkplaceLifeComponent
{
    private PieceReproductionDef def;
    
    private string childName;
    public override string AnalyticsLocation => $"skip_product{(TimerCooldown.IsExecuteable() ? "_cooldown" : "")}";
    public override string Message => string.Format(LocalizationService.Get("gameboard.bubble.message.production", "gameboard.bubble.message.production {0}"), childName);
    public override string Price => string.Format(LocalizationService.Get("gameboard.bubble.button.wait", "gameboard.bubble.button.wait\n{0}"), TimerCooldown.CompleteTime.GetTimeLeftText());
    
    public override CurrencyPair Worker => null;
    public override bool IsCanUseExtraWorker => false;

    public override TimerComponent TimerMain => TimerCooldown;
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        base.OnRegisterEntity(entity);
        
        def = GameDataService.Current.PiecesManager.GetPieceDef(Context.PieceType).ReproductionDef;

        Context.TutorialLocker?.SetTouchAction(() =>
        {
            var view = Context.ViewDefinition.AddView(ViewType.Bubble) as BubbleView;

            if (view.IsShow == false)
            {
                view.SetData(
                    LocalizationService.Get("common.message.unavailable.candy", "common.message.unavailable.candy"),
                    LocalizationService.Get("common.button.ok", "common.button.ok"),
                    (p) => { view.Change(false); },
                    true,
                    false
                );
            }
        
            view.Change(!view.IsShow);
        });
        
        Context.TutorialLocker?.SetCompleteAction(() =>
        {
            var view = Context.ActorView as ReproductionPieceView;
            if(view != null) view.ToggleEffectsByState(false);
            Rewards.ShowBubble();
        });
        
        HP = def.Limit;
        TimerWork.Delay = 0;
        
        TimerCooldown = new TimerComponent{Delay = def.Delay};
        TimerCooldown.OnComplete += TimerWork.Start;
        RegisterComponent(TimerCooldown);
        
        var child = GameDataService.Current.PiecesManager.GetPieceDef(PieceType.Parse(def.ReproductionId));
        childName = $"<sprite name={child.Uid}>";
    }

    public override void OnMovedFromToFinish(BoardPosition @from, BoardPosition to, Piece context = null)
    {
    }

    public override void OnRemoveFromBoard(BoardPosition position, Piece context = null)
    {
        TimerWork.OnComplete -= TimerWork.Start;
        base.OnRemoveFromBoard(position, context);
    }
    
    protected override LifeSaveItem InitInSave(BoardPosition position)
    {
        Rewards.InitInSave(position);
		
        var save = ProfileService.Current.GetComponent<FieldDefComponent>(FieldDefComponent.ComponentGuid);
        var item = save?.GetLifeSave(position);

        if (item == null)
        {
            OnTimerStart();
            if (Context.TutorialLocker == null) Rewards.ShowBubble();
            return null;
        }
		
        current = item.Step;
        
        if (item.IsStartCooldown) TimerCooldown.Start(item.StartTimeCooldown);
        else
        {
            SetStepReward();
            if (Context.TutorialLocker == null) Rewards.ShowBubble();
        }

        return item;
    }
    
    public override LifeSaveItem Save()
    {
        return current == 0 ? null : new LifeSaveItem
        {
            Step = current,
            Position = Context.CachedPosition,
            IsStartCooldown = TimerCooldown.IsExecuteable(),
            StartTimeCooldown = TimerCooldown.StartTimeLong
        };
    }
    
    public override bool Damage(bool isExtra = false)
    {
        if (TimerCooldown.IsExecuteable())
        {
            UIMessageWindowController.CreateTimerCompleteMessage(
                LocalizationService.Get("window.timerComplete.message.production", "window.timerComplete.message.production"),
                AnalyticsLocation,
                PieceType.Parse(Context.PieceType),
                TimerCooldown);
            
            return false;
        }
        
        if (IsDead || CurrencyHelper.IsCanPurchase(Energy, true) == false) return false;
        
        CurrencyHelper.Purchase(Currency.Damage.Name, 1, Energy, success =>
        {
            Success();
            Damage(1);
            
            BoardService.Current.FirstBoard.BoardEvents.RaiseEvent(GameEventsCodes.StorageDamage, this);
        });
        
        return true;
    }

    protected override void OnTimerStart()
    {
        Damage();
        SetStepReward();
    }

    private void SetStepReward()
    {
        if (IsDead == false) OnStep();
        else OnComplete();
        
        PlaySoundOnStart();
		
        Locker.Lock(this, false);
    }

    protected override void OnTimerComplete()
    {
        PlaySoundOnEnd();
        Rewards.ShowBubble();
    }
    
    protected override Dictionary<int, int> GetRewards()
    {
        var pieces = new Dictionary<int, int>();
        
        if (IsDead) pieces.Add(def.ObstacleType, 1);
        
        pieces.Add(PieceType.Parse(def.ReproductionId), def.ReproductionRange.Range());
        
        return pieces;
    }

    protected override void OnSpawnCurrencyRewards(bool isComplete)
    {
        if (isComplete)
        {
            AddResourceView.Show(StartPosition(), def.StepReward);
            if (IsDead == false) TimerCooldown.Start();
        }
        
        base.OnSpawnCurrencyRewards(isComplete);
    }
}