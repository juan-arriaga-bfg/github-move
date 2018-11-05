using System.Collections.Generic;

public class MineLifeComponent : StorageLifeComponent
{
    private MineDef def;
    
    public override CurrencyPair Energy => def.Price;
    public override string Message => $"Clear mine:\n{DateTimeExtension.GetDelayText(def.Delay)}";
    public override string Price => $"Clear {Energy.ToStringIcon()}";
    
    public int MinePieceType { get; private set; }
    
    public override void OnAddToBoard(BoardPosition position, Piece context = null)
    {
        MinePieceType = context.PieceType;
        
        var key = new BoardPosition(position.X, position.Y);

        if (def == null) def = GameDataService.Current.MinesManager.GetInitialDef(key);
        else GameDataService.Current.MinesManager.Move(def.Id, key);
        
        storage.Timer.Delay = def.Delay;
        
        HP = def.Size;
        
        base.OnAddToBoard(position, context);
    }

    public override void OnMovedFromToFinish(BoardPosition @from, BoardPosition to, Piece context = null)
    {
        base.OnMovedFromToFinish(@from, to, context);
        
        var key = new BoardPosition(to.X, to.Y);
        GameDataService.Current.MinesManager.Move(def.Id, key);
    }

    protected override void InitStorage()
    {
        storage.SpawnPiece = PieceType.Parse(def.Reward.Currency);
        storage.Capacity = storage.Amount = def.Reward.Amount;
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
        if(Reward == null || Reward.Count == 0) Reward = GameDataService.Current.MinesManager.GetSequence(def.Skin).GetNextDict(def.PieceAmount);
        
        storage.SpawnAction = new EjectionPieceAction
        {
            GetFrom = () => thisContext.CachedPosition,
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
        AddResourceView.Show(StartPosition(), def.StepRewards);
        base.OnSpawnRewards();
    }

    protected override void OnTimerComplete()
    {
        base.OnTimerComplete();
        
        GameDataService.Current.QuestsManager.CheckConditions(); // To ensure that QuestStartConditionMineUsedComponent triggered
        BoardService.Current.FirstBoard.BoardEvents.RaiseEvent(GameEventsCodes.MineUsed, this);
    }
}