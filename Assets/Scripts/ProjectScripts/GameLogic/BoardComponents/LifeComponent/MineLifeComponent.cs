using System.Collections.Generic;

public class MineLifeComponent : WorkplaceLifeComponent
{
    private MineDef def;
    
    public override CurrencyPair Energy => def.Price;
    public override string Message => string.Format(LocalizationService.Get("gameboard.bubble.message.mine", "gameboard.bubble.message.mine\n{0}?"), DateTimeExtension.GetDelayText(def.Delay));
    public override string Price => string.Format(LocalizationService.Get("gameboard.bubble.button.clear", "gameboard.bubble.button.clear {0}"), Energy.ToStringIcon());
    
    public int MinePieceType { get; private set; }
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        base.OnRegisterEntity(entity);
        
        MinePieceType = Context.PieceType;
    }
    
    public override void OnAddToBoard(BoardPosition position, Piece context = null)
    {
        var key = new BoardPosition(position.X, position.Y);

        if (def == null) def = GameDataService.Current.MinesManager.GetInitialDef(key);
        else GameDataService.Current.MinesManager.Move(def.Id, key);
        
        Timer.Delay = def.Delay;
        HP = def.Size;
        
        base.OnAddToBoard(position, context);
    }

    public override void OnMovedFromToFinish(BoardPosition @from, BoardPosition to, Piece context = null)
    {
        base.OnMovedFromToFinish(@from, to, context);
        
        var key = new BoardPosition(to.X, to.Y);
        GameDataService.Current.MinesManager.Move(def.Id, key);
    }
    
    protected override Dictionary<int, int> GetRewards()
    {
        var pieces = new Dictionary<int, int> {{PieceType.Parse(def.Reward.Currency), def.Reward.Amount}};
        
        if (IsDead == false) return pieces;
        
        foreach (var pair in def.LastRewards)
        {
            pieces.Add(PieceType.Parse(pair.Currency), pair.Amount);
        }
        
        return pieces;
    }
    
    protected override void OnComplete()
    {
        base.OnComplete();
        
        Rewards.OnComplete = () =>
        {
            GameDataService.Current.MinesManager.Remove(def.Id);
            OnSpawnCurrencyRewards();
        };
    }

    protected override void OnSpawnCurrencyRewards()
    {
        AddResourceView.Show(StartPosition(), def.StepRewards);
        base.OnSpawnCurrencyRewards();
    }

    protected override void OnTimerComplete()
    {
        base.OnTimerComplete();
        
        GameDataService.Current.QuestsManager.StartNewQuestsIfAny(); // To ensure that QuestStartConditionMineUsedComponent triggered
        BoardService.Current.FirstBoard.BoardEvents.RaiseEvent(GameEventsCodes.MineUsed, this);
    }
}