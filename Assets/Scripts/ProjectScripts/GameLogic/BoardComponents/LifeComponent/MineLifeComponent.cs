using System.Collections.Generic;

public class MineLifeComponent : WorkplaceLifeComponent
{
    private PieceMineDef def;
    
    public override CurrencyPair Energy => def.Price;
    public override string Message => string.Format(LocalizationService.Get("gameboard.bubble.message.mine", "gameboard.bubble.message.mine\n{0}?"), DateTimeExtension.GetDelayText(def.Delay));
    public override string Price => string.Format(LocalizationService.Get("gameboard.bubble.button.clear", "gameboard.bubble.button.clear {0}"), Energy.ToStringIcon());
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        base.OnRegisterEntity(entity);
        
        def = GameDataService.Current.PiecesManager.GetPieceDef(Context.PieceType).MineDef;
        
        TimerMain.Delay = def.Delay;
        HP = def.Size;
    }

    protected override LifeSaveItem InitInSave(BoardPosition position)
    {
        var item = base.InitInSave(position);
        
        if (item != null && item.IsStartTimer == false) Locker.Unlock(this);
        
        return item;
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

    protected override void OnSpawnCurrencyRewards(bool isComplete)
    {
        if (isComplete)
        {
            AddResourceView.Show(StartPosition(), def.StepRewards);
            
            //if (IsDead) GameDataService.Current.MinesManager.Remove(def.Id);
        }
        
        base.OnSpawnCurrencyRewards(isComplete);
    }

    protected override void OnTimerComplete()
    {
        base.OnTimerComplete();
        
        GameDataService.Current.QuestsManager.StartNewQuestsIfAny(); // To ensure that QuestStartConditionMineUsedComponent triggered
        BoardService.Current.FirstBoard.BoardEvents.RaiseEvent(GameEventsCodes.MineUsed, this);
    }
}