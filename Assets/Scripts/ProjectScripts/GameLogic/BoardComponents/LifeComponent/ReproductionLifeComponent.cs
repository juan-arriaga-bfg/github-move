using System.Collections.Generic;

public class ReproductionLifeComponent : WorkplaceLifeComponent
{
    private PiecesReproductionDef def;
    private TimerComponent cooldown;
    
    private string childName;
    
    public override string Message => string.Format(LocalizationService.Get("gameboard.bubble.message.production", "gameboard.bubble.message.production {0}"), childName);
    public override string Price => Timer.IsExecuteable() ? string.Format(LocalizationService.Get("gameboard.bubble.button.wait", "gameboard.bubble.button.wait\n{0}"), Timer.CompleteTime.GetTimeLeftText()) : base.Price;

    public override TimerComponent Timer => cooldown;

    public override bool IsUseCooldown => true;

    public override void OnRegisterEntity(ECSEntity entity)
    {
        base.OnRegisterEntity(entity);
        
        def = GameDataService.Current.PiecesManager.GetPieceDef(Context.PieceType).ReproductionDef;
        
        HP = def.Limit;
        timer.Delay = 2;
        
        cooldown = new TimerComponent{Delay = def.Delay};
        RegisterComponent(cooldown);
        
        var child = GameDataService.Current.PiecesManager.GetPieceDef(PieceType.Parse(def.Reproduction.Currency));
        childName = child?.Name;
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
        
        if (item.IsStartCooldown) cooldown.Start(item.StartTimeCooldown);
        else Locker.Unlock(this);
        
        return item;
    }
    
    public override LifeSaveItem Save()
    {
        var save = base.Save();
        
        if (save == null) return null;
        
        save.IsStartCooldown = cooldown.IsExecuteable();
        save.StartTimeCooldown = cooldown.StartTimeLong;
        
        return save;
    }

    public override bool Damage(bool isExtra = false)
    {
        if (cooldown.IsExecuteable() == false) return base.Damage(isExtra);
        
        UIMessageWindowController.CreateTimerCompleteMessage(
            LocalizationService.Get("window.timerComplete.message.production", "window.timerComplete.message.production"),
            cooldown);
        
        return false;
    }

    protected override void OnSpawnCurrencyRewards()
    {
        AddResourceView.Show(StartPosition(), def.StepReward);
        base.OnSpawnCurrencyRewards();
        if (IsDead == false) cooldown.Start();
    }
}