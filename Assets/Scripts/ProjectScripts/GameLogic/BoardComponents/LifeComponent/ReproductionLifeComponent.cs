using System.Collections.Generic;

public class ReproductionLifeComponent : StorageLifeComponent
{
    private PiecesReproductionDef def;
    private string childName;
    private TimerComponent cooldown;
    
    public override string Message => string.Format(LocalizationService.Instance.Manager.GetTextByUid("gameboard.bubble.message.production", "Harvest {0}"), childName);
    public override string Price => Timer.IsExecuteable() ? string.Format(LocalizationService.Instance.Manager.GetTextByUid("gameboard.bubble.button.wait", "Wait\n{0}"), Timer.CompleteTime.GetTimeLeftText()) : base.Price;

    public override TimerComponent Timer => cooldown;

    public override bool IsUseCooldown => true;

    public override void OnRegisterEntity(ECSEntity entity)
    {
        base.OnRegisterEntity(entity);
        
        def = GameDataService.Current.PiecesManager.GetPieceDef(thisContext.PieceType).ReproductionDef;
        
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
                LocalizationService.Get("window.timerComplete.message.production", "Would you like to build the item right now for crystals?"),
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
            thisContext.Context.ActionExecutor.AddAction(new CollapsePieceToAction
            {
                To = thisContext.CachedPosition,
                Positions = new List<BoardPosition> {thisContext.CachedPosition},
                OnComplete = OnRemove
            });
        };
    }

    private void OnRemove()
    {
        thisContext.Context.ActionExecutor.AddAction(new SpawnPieceAtAction()
        {
            IsCheckMatch = false,
            At = thisContext.CachedPosition,
            PieceTypeId = def.ObstacleType
        });
    }

    protected override void OnSpawnRewards()
    {
        base.OnSpawnRewards();
        AddResourceView.Show(StartPosition(), def.StepReward);
    }
}