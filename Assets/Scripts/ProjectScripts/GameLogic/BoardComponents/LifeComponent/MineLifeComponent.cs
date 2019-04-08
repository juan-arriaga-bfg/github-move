using System.Collections.Generic;
using UnityEngine;

public class MineLifeComponent : WorkplaceLifeComponent
{
    private PieceMineDef def;
    
    public override CurrencyPair Energy => def.Price;
    public override string AnalyticsLocation => $"skip_mine";
    public override string Message => LocalizationService.Get("gameboard.bubble.message.mine", "gameboard.bubble.message.mine");
    public override string Price => string.Format(LocalizationService.Get("gameboard.bubble.button.clear", "gameboard.bubble.button.clear {0}"), Energy.ToStringIcon());
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        base.OnRegisterEntity(entity);
        def = GameDataService.Current.PiecesManager.GetPieceDef(Context.PieceType).MineDef;
        
        TimerWork.Delay = WorkerCurrencyLogicComponent.MinDelay;
        HP = def.Size;
    }

    protected override LifeSaveItem InitInSave(BoardPosition position)
    {
        Rewards.InitInSave(position);
		
        var save = ProfileService.Current.GetComponent<FieldDefComponent>(FieldDefComponent.ComponentGuid);
        var item = save?.GetLifeSave(position);
        
        if (item == null) return null;
		
        current = item.Step;
        Context.Context.WorkerLogic.Init(Context.CachedPosition, TimerWork);
        
        TimerWork.IsCanceled = TimerWork.Delay > WorkerCurrencyLogicComponent.MinDelay;
        
        if (item.IsStartTimer) TimerWork.Start(item.StartTimeTimer);
        else
        {
            OnTimerStart();
            if (Rewards.IsComplete == false) Locker.Unlock(this);
        }
        
        return item;
    }
    
    public override bool Damage(bool isExtra = false)
    {
        var isDamage = base.Damage(isExtra);

        if (isDamage) (Context.ActorView as MinePieceView)?.PlayWorkAnimation();
        
        return isDamage;
    }

    protected override void OnComplete()
    {
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
            
            if (IsDead)
            {
                var action = Context.Context.BoardLogic.MatchActionBuilder.GetMatchAction(new List<BoardPosition>{Context.CachedPosition}, Context.PieceType, Context.CachedPosition);

                if (action == null) return;
                
                Context.Context.ActionExecutor.AddAction(action);
                
                var data = GameDataService.Current.PiecesManager.GetComponent<PiecesMineDataManager>(PiecesMineDataManager.ComponentGuid);
                data.DecrementLoop(def.Id);
            }
        }
        
        base.OnSpawnCurrencyRewards(isComplete);
    }
    
    protected override void OnTimerComplete()
    {
        base.OnTimerComplete();
        (Context.ActorView as MinePieceView)?.CompleteWorkAnimation();
        GameDataService.Current.QuestsManager.StartNewQuestsIfAny(); // To ensure that QuestStartConditionMineUsedComponent triggered
        BoardService.Current.FirstBoard.BoardEvents.RaiseEvent(GameEventsCodes.MineUsed, this);
    }
}