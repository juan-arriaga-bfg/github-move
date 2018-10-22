using System.Collections.Generic;

public class MineLifeComponent : StorageLifeComponent
{
    private MineDef def;
    
    public override CurrencyPair Energy => def.Price;
    public override string Message => $"Clear mine:\n{DateTimeExtension.GetDelayText(def.Delay)}";
    public override string Price => $"Clear {Energy.ToStringIcon()}";
    
    public override void OnAddToBoard(BoardPosition position, Piece context = null)
    {
        base.OnAddToBoard(position, context);
        
        var key = new BoardPosition(position.X, position.Y);

        if (def == null) def = GameDataService.Current.MinesManager.GetInitialDef(key);
        else GameDataService.Current.MinesManager.Move(def.Id, key);
        
        var timer = thisContext.GetComponent<TimerComponent>(TimerComponent.ComponentGuid);
        
        timer.Delay = def.Delay;
        timer.Price = def.FastPrice;
        
        storage.SpawnPiece = PieceType.Parse(def.Reward.Currency);
        storage.Capacity = storage.Amount = def.Reward.Amount;
        
        HP = def.Size;
    }

    public override void OnMovedFromToFinish(BoardPosition @from, BoardPosition to, Piece context = null)
    {
        base.OnMovedFromToFinish(@from, to, context);
        
        var key = new BoardPosition(to.X, to.Y);
        GameDataService.Current.MinesManager.Move(def.Id, key);
    }

    protected override void OnStep()
    {
        storage.OnScatter = () =>
        {
            storage.OnScatter = null;
            OnSpawnRewards();
        };
    }

    protected override void OnComplete()
    {
        var position = thisContext.CachedPosition;
                
        storage.OnScatter = () =>
        {
            storage.OnScatter = null;
            OnSpawnRewards();
            thisContext.Context.ActionExecutor.AddAction(new CollapsePieceToAction
            {
                To = position,
                Positions = new List<BoardPosition> {position},
                OnComplete = OnRemove
            });
        };
    }

    private void OnRemove()
    {
        GameDataService.Current.MinesManager.Remove(def.Id);
        
        var multi = thisContext.GetComponent<MulticellularPieceBoardObserver>(MulticellularPieceBoardObserver.ComponentGuid);
        
        if(multi == null) return;

        var mask = multi.Mask;
        
        foreach (var maskPoint in mask)
        {
            var point = multi.GetPointInMask(thisContext.CachedPosition, maskPoint);
            
            thisContext.Context.ActionExecutor.AddAction(new SpawnPieceAtAction()
            {
                IsCheckMatch = false,
                At = point,
                PieceTypeId = PieceType.OX1.Id
            });
        }
    }

    protected override void OnSpawnRewards()
    {
        AddResourceView.Show(StartPosition(), def.StepReward);
        base.OnSpawnRewards();
    }

    protected override void OnTimerComplete()
    {
        base.OnTimerComplete();
        
        GameDataService.Current.QuestsManager.CheckConditions(); // To ensure that QuestStartConditionMineUsedComponent triggered
        BoardService.Current.FirstBoard.BoardEvents.RaiseEvent(GameEventsCodes.MineUsed, this);
    }
}