using System.Collections.Generic;

public class MakingLifeComponent : WorkplaceLifeComponent
{
    private PiecesMakingDef def;
    private TimerComponent cooldown;
    
    public override CurrencyPair Energy => def.Price;
    public override string Message => string.Format(LocalizationService.Get("gameboard.bubble.message.castle.make", "gameboard.bubble.message.castle.make {0}"), Energy.ToStringIcon());
    public override string Price => Timer.IsExecuteable() ? string.Format(LocalizationService.Get("gameboard.bubble.button.wait", "gameboard.bubble.button.wait\n{0}"), Timer.CompleteTime.GetTimeLeftText()) : base.Price;

    public override TimerComponent Timer => cooldown;

    public override bool IsUseCooldown => true;

    public override void OnRegisterEntity(ECSEntity entity)
    {
        base.OnRegisterEntity(entity);
        
        def = GameDataService.Current.PiecesManager.GetPieceDef(Context.PieceType).MakingDef;
        
        HP = -1;
        timer.Delay = 2;
        
        cooldown = new TimerComponent{Delay = def.Delay};
        RegisterComponent(cooldown);
    }

    protected override Dictionary<int, int> GetRewards()
    {
        return GameDataService.Current.PiecesManager.GetSequence(def.Uid).GetNextDict(def.PieceAmount);
    }
    
    protected override void InitInSave(BoardPosition position)
    {
        Locker.Unlock(this);
        base.InitInSave(position);
    }
    
    public override bool Damage(bool isExtra = false)
    {
        if (cooldown.IsExecuteable() == false) return base.Damage(isExtra);
        
        UIMessageWindowController.CreateTimerCompleteMessage(
            LocalizationService.Get("window.timerComplete.message.castle", "window.timerComplete.message.castle"),
            cooldown);

        return false;
    }
    
    protected override void OnSpawnCurrencyRewards()
    {
        AddResourceView.Show(StartPosition(), def.StepRewards);
        base.OnSpawnCurrencyRewards();
        cooldown.Start();
    }
}