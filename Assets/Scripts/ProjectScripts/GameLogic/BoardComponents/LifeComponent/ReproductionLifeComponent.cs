using System.Collections.Generic;

public class ReproductionLifeComponent : StorageLifeComponent
{
    private PiecesReproductionDef def;
    private string childName;
    private TimerComponent cooldown;
    
    public override string Message => $"Harvest {childName}";
    public override string Price => Timer.IsExecuteable() ? $"Wait\n{Timer.CompleteTime.GetTimeLeftText()}" : base.Price;

    public override TimerComponent Timer => cooldown;

    public override bool IsUseCooldown => true;

    public override void OnRegisterEntity(ECSEntity entity)
    {
        base.OnRegisterEntity(entity);
        
        def = GameDataService.Current.PiecesManager.GetPieceDef(thisContext.PieceType).ReproductionDef;
        
        cooldown = new TimerComponent{Delay = def.Delay, Price = def.FastPrice};
        RegisterComponent(cooldown);
        
        var child = GameDataService.Current.PiecesManager.GetPieceDef(PieceType.Parse(def.Reproduction.Currency));
        childName = child?.Name;
    }

    public override void OnAddToBoard(BoardPosition position, Piece context = null)
    {
        base.OnAddToBoard(position, context);
        
        var timer = thisContext.GetComponent<TimerComponent>(TimerComponent.ComponentGuid);
        
        timer.Delay = 2;
        
        storage.SpawnPiece = PieceType.Parse(def.Reproduction.Currency);
        storage.Capacity = storage.Amount = def.Reproduction.Amount;
        
        HP = def.Limit;
    }

    protected override LifeSaveItem InitInSave(BoardPosition position)
    {
        var item = base.InitInSave(position);
        
        if(item != null) cooldown.Start(item.StartTime);
        
        return item;
    }

    public override bool Damage()
    {
        if (!cooldown.IsExecuteable()) return base.Damage();
        
        UIMessageWindowController.CreateTimerCompleteMessage(
            "Complete now!",
            "Would you like to build the item right now for crystals?",
            "Complete now ",
            cooldown,
            () => CurrencyHellper.Purchase(Currency.Timer.Name, 1, cooldown.GetPrise(), success =>
            {
                if(success == false) return;
					
                cooldown.Stop();
                cooldown.OnComplete();
            }));
            
        return false;

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
            PieceTypeId = PieceType.OX1.Id
        });
    }

    protected override void OnSpawnRewards()
    {
        AddResourceView.Show(StartPosition(), def.StepReward);
    }
}