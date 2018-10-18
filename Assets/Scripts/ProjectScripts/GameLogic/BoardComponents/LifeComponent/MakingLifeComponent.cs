public class MakingLifeComponent : StorageLifeComponent
{
    private PiecesMakingDef def;
    private TimerComponent cooldown;
    
    public override CurrencyPair Energy => def.Price;
    public override string Message => $"Make: {Energy.ToStringIcon()}";
    public override string Price => Timer.IsExecuteable() ? $"Wait\n{Timer.CompleteTime.GetTimeLeftText()}" : base.Price;

    public override TimerComponent Timer => cooldown;

    public override bool IsUseCooldown => true;

    public override void OnRegisterEntity(ECSEntity entity)
    {
        base.OnRegisterEntity(entity);
        
        def = GameDataService.Current.PiecesManager.GetPieceDef(thisContext.PieceType).MakingDef;
        
        HP = -1;
        cooldown = new TimerComponent{Delay = def.Delay, Price = def.FastPrice};
        RegisterComponent(cooldown);
    }

    public override void OnAddToBoard(BoardPosition position, Piece context = null)
    {
        base.OnAddToBoard(position, context);
        
        var timer = thisContext.GetComponent<TimerComponent>(TimerComponent.ComponentGuid);
        
        timer.Delay = 2;
        
        storage.Capacity = storage.Amount = def.PieceAmount;
    }

    protected override LifeSaveItem InitInSave(BoardPosition position)
    {
        var item = base.InitInSave(position);
        
        if (item != null && item.IsStart) cooldown.Start(item.StartTime);
        
        return item;
    }

    public override bool Damage()
    {
        if (cooldown.IsExecuteable())
        {
            UIMessageWindowController.CreateTimerCompleteMessage(
                "Complete now!",
                "Would you like to build the item right now for crystals?",
                cooldown);

            return false;
        }
        
        return base.Damage();
    }

    protected override void Success()
    {
    }

    protected override void OnStep()
    {
        var pieces = ItemWeight.GetRandomPieces(def.PieceAmount, def.PieceWeights);
        
        foreach (var key in pieces.Keys)
        {
            storage.SpawnPiece = key;
            break;
        }
        
        storage.SpawnAction = new EjectionPieceAction
        {
            From = thisContext.CachedPosition,
            Pieces = pieces,
            OnComplete = OnSpawnRewards
        };
    }
    
    protected override void OnSpawnRewards()
    {
        AddResourceView.Show(StartPosition(), def.StepRewards);
        cooldown.Start();
    }
}