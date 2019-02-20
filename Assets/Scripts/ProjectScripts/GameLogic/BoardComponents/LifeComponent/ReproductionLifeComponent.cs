using System.Collections.Generic;

public class ReproductionLifeComponent : WorkplaceLifeComponent
{
    private PieceReproductionDef def;
    
    private string childName;
    public override string AnalyticsLocation => $"skip_product{(TimerCooldown.IsExecuteable() ? "_cooldown" : "")}";
    public override string Message => string.Format(LocalizationService.Get("gameboard.bubble.message.production", "gameboard.bubble.message.production {0}"), childName);
    public override string Price => TimerCooldown.IsExecuteable()
        ? string.Format(LocalizationService.Get("gameboard.bubble.button.wait", "gameboard.bubble.button.wait\n{0}"), TimerCooldown.CompleteTime.GetTimeLeftText())
        : string.Format(LocalizationService.Get("gameboard.bubble.button.send", "gameboard.bubble.button.send {0}"), string.Empty);
    
    public override CurrencyPair Worker => null;

    public override TimerComponent TimerMain => TimerCooldown;
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        base.OnRegisterEntity(entity);
        
        def = GameDataService.Current.PiecesManager.GetPieceDef(Context.PieceType).ReproductionDef;
        
        HP = def.Limit;
        TimerWork.Delay = 0;
        
        TimerCooldown = new TimerComponent{Delay = def.Delay};
        RegisterComponent(TimerCooldown);
        
        var child = GameDataService.Current.PiecesManager.GetPieceDef(PieceType.Parse(def.Reproduction.Currency));
        childName = $"<sprite name={child.Uid}>";
    }
    
    protected override Dictionary<int, int> GetRewards()
    {
        var pieces = new Dictionary<int, int>();
        
        if (IsDead) pieces.Add(def.ObstacleType, 1);
        
        pieces.Add(PieceType.Parse(def.Reproduction.Currency), def.Reproduction.Amount);
        
        return pieces;
    }
    
    protected override LifeSaveItem InitInSave(BoardPosition position)
    {
        var item = base.InitInSave(position);
        
        if (item == null) return null;
        
        if (item.IsStartCooldown) TimerCooldown.Start(item.StartTimeCooldown);
        else Locker.Unlock(this);
        
        return item;
    }
    
    public override LifeSaveItem Save()
    {
        var save = base.Save();
        
        if (save == null) return null;
        
        save.IsStartCooldown = TimerCooldown.IsExecuteable();
        save.StartTimeCooldown = TimerCooldown.StartTimeLong;
        
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