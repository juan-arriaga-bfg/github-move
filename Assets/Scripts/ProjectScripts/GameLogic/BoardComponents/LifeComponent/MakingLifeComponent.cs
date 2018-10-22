﻿public class MakingLifeComponent : StorageLifeComponent
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
        
        cooldown.OnComplete += Unlock;
    }

    public override void OnAddToBoard(BoardPosition position, Piece context = null)
    {
        var timer = thisContext.GetComponent<TimerComponent>(TimerComponent.ComponentGuid);
        
        timer.Delay = 2;
        
        if (storage == null)
        {
            storage = thisContext.GetComponent<StorageComponent>(StorageComponent.ComponentGuid);
        }
        storage.Capacity = storage.Amount = def.PieceAmount;
        
        // Called at the end because InitInSave called from OnAddToBoard should have storage.Capacity initialized
        base.OnAddToBoard(position, context);
    }
    
    public override void OnRemoveFromBoard(BoardPosition position, Piece context = null)
    {
        base.OnRemoveFromBoard(position, context);
        cooldown.OnComplete -= Unlock;
    }

    protected override LifeSaveItem InitInSave(BoardPosition position)
    {
        var item = base.InitInSave(position);
        
        if (item != null && item.IsStart) cooldown.Start(item.StartTime);
        
        return item;
    }

    public override bool Damage(bool isExtra = false)
    {
        if (cooldown.IsExecuteable())
        {
            UIMessageWindowController.CreateTimerCompleteMessage(
                "Complete now!",
                "Would you like to build the item right now for crystals?",
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
    
    private void Unlock()
    {
        Locker.Unlock(this);
    }
}