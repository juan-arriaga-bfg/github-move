using System.Collections.Generic;

public class ReproductionLifeComponent : StorageLifeComponent
{
    private PiecesReproductionDef def;
    private string childName;
    private TimerComponent cooldown;
    
    public override string Message => string.Format(LocalizationService.Get("gameboard.bubble.message.production", "gameboard.bubble.message.production {0}"), childName);
    public override string Price => Timer.IsExecuteable() ? string.Format(LocalizationService.Get("gameboard.bubble.button.wait", "gameboard.bubble.button.wait\n{0}"), Timer.CompleteTime.GetTimeLeftText()) : base.Price;

    public override TimerComponent Timer => cooldown;

    public override bool IsUseCooldown => true;

    public override void OnRegisterEntity(ECSEntity entity)
    {
        base.OnRegisterEntity(entity);
        
        def = GameDataService.Current.PiecesManager.GetPieceDef(Context.PieceType).ReproductionDef;
        
        HP = def.Limit;
        cooldown = new TimerComponent{Delay = def.Delay};
        RegisterComponent(cooldown);
        
        var child = GameDataService.Current.PiecesManager.GetPieceDef(PieceType.Parse(def.Reproduction.Currency));
        childName = child?.Name;
    }
    
    protected override void InitStorage()
    {
        storage.SpawnPiece = PieceType.Parse(def.Reproduction.Currency);
        storage.Capacity = storage.Amount = def.Reproduction.Amount;
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
                LocalizationService.Get("window.timerComplete.message.production", "window.timerComplete.message.production"),
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
        storage.OnScatter = () =>
        {
            storage.OnScatter = null;
            OnSpawnRewards();
            cooldown.Start();
        };
    }

    protected override void OnComplete()
    {
        storage.OnScatter = () =>
        {
            storage.OnScatter = null;
            OnSpawnRewards();
            Context.Context.ActionExecutor.AddAction(new CollapsePieceToAction
            {
                To = Context.CachedPosition,
                Positions = new List<BoardPosition> {Context.CachedPosition},
                OnComplete = OnRemove
            });
        };
    }

    private void OnRemove()
    {
        Context.Context.ActionExecutor.AddAction(new SpawnPieceAtAction()
        {
            IsCheckMatch = false,
            At = Context.CachedPosition,
            PieceTypeId = def.ObstacleType
        });
    }

    protected override void OnSpawnRewards()
    {
        base.OnSpawnRewards();
        AddResourceView.Show(StartPosition(), def.StepReward);
    }
}