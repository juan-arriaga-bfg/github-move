public class MakingLifeComponent : StorageLifeComponent
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
        
        cooldown = new TimerComponent{Delay = def.Delay};
        RegisterComponent(cooldown);
    }
    
    protected override void InitStorage()
    {
        storage.Capacity = storage.Amount = def.PieceAmount;
        storage.Timer.Delay = 2;
    }

    protected override LifeSaveItem InitInSave(BoardPosition position)
    {
        var item = base.InitInSave(position);
        
        if (item == null) return null;
        
        if(item.IsStart) cooldown.Start(item.StartTime);
        Locker.Unlock(this);
        
        return item;
    }

    public override bool Damage(bool isExtra = false)
    {
        if (cooldown.IsExecuteable())
        {
            UIMessageWindowController.CreateTimerCompleteMessage(
                LocalizationService.Get("indow.timerComplete.message.castle", "indow.timerComplete.message.castle"),
                cooldown);

            return false;
        }
        
        return base.Damage(isExtra);
    }

    protected override void Success()
    {
    }

    protected override void OnStep()
    {
        if(Reward == null || Reward.Count == 0) Reward = GameDataService.Current.PiecesManager.GetSequence(def.Uid).GetNextDict(def.PieceAmount);
        
        foreach (var key in Reward.Keys)
        {
            storage.SpawnPiece = key;
            break;
        }
        
        storage.SpawnAction = new EjectionPieceAction
        {
            GetFrom = () => Context.CachedPosition,
            Pieces = Reward,
            OnComplete = () =>
            {
                Reward = null;
                OnSpawnRewards();
            }
        };
    }
    
    protected override void OnSpawnRewards()
    {
        base.OnSpawnRewards();
        AddResourceView.Show(StartPosition(), def.StepRewards);
        cooldown.Start();
    }
}