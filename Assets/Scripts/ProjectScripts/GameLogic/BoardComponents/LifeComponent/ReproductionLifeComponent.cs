﻿using System.Collections.Generic;

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
        
        HP = def.Limit;
        cooldown = new TimerComponent{Delay = def.Delay, Price = def.FastPrice};
        RegisterComponent(cooldown);

        cooldown.OnComplete += Unlock;
        
        var child = GameDataService.Current.PiecesManager.GetPieceDef(PieceType.Parse(def.Reproduction.Currency));
        childName = child?.Name;
    }

    public override void OnAddToBoard(BoardPosition position, Piece context = null)
    {
        var timer = thisContext.GetComponent<TimerComponent>(TimerComponent.ComponentGuid);
        
        timer.Delay = 2;
        
        if (storage == null)
        {
            storage = thisContext.GetComponent<StorageComponent>(StorageComponent.ComponentGuid);
        }
        storage.SpawnPiece = PieceType.Parse(def.Reproduction.Currency);
        storage.Capacity = storage.Amount = def.Reproduction.Amount;
        
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
        AddResourceView.Show(StartPosition(), def.StepReward);
    }

    private void Unlock()
    {
        Locker.Unlock(this);
    }
}