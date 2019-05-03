using System;
using System.Collections.Generic;
using UnityEngine;

public enum EventGameType
{
    OrderSoftLaunch,
}

public enum EventGameState
{
    Default,
    Preview,
    InProgress,
    Complete,
    Claimed
}

public class EventGame : ECSEntity
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;
    
    public EventGameType EventType;
    public EventGameState State;

    public List<EventGameStepDef> Steps;
    
    public int Step => ProfileService.Current.GetStorageItem(Currency.EventStep.Name).Amount;
    
    public int Price
    {
        get
        {
            var step = Mathf.Min(Step, Steps.Count - 1);
            return Steps[step].Prices[0].Amount;
        }
    }
    
    public bool IsPremium => false;
    
    public bool IsLastStep => Step == Steps.Count - 1;

    public bool IsCompleted => Step == Steps.Count;
    
    private EventGamesLogicComponent context;

    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public int IntroDuration { get; private set; }
    
    public long StartTimeLong => StartTime.ConvertToUnixTime();
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        context = entity as EventGamesLogicComponent;
    }

    public void InitData(DateTime start, DateTime end, int intro)
    {
        StartTime = start;
        EndTime = end;
        IntroDuration = intro;
    }

    public void UpdateData(DateTime end, int intro)
    {
        EndTime = end;
        IntroDuration = intro;
    }
    
    public void Finish()
    {
        RemovePieces();

        if (Step == Steps.Count)
        {
            RemoveCurrency();
            return;
        }
        
        var stepDef = Steps[Step];
        var model = UIService.Get.GetCachedModel<UIEventAlmostWindowModel>(UIWindowType.EventAlmostWindow);
        
        RemoveCurrency();
        
        model.Price = null;
        model.Rewards = new List<CurrencyPair>();

        if (stepDef.IsNormalIgnored == false)
        {
            model.Price = stepDef.NormalRewardsPrice.Copy();
            model.Rewards.AddRange(stepDef.NormalRewards);
        }
        
        if (IsPremium && stepDef.IsPremiumIgnored == false)
        {
            if (model.Price == null) model.Price = stepDef.PremiumRewardsPrice.Copy();
            else model.Price.Amount += stepDef.PremiumRewardsPrice.Amount;
            
            model.Rewards.AddRange(stepDef.PremiumRewards);
        }
        
        if (model.Price == null) return;
        
        UIService.Get.ShowWindow(UIWindowType.EventAlmostWindow);
    }

    private void RemovePieces()
    {
        var board = BoardService.Current.FirstBoard;
        var positions = new List<BoardPosition>();

        for (var id = PieceType.Token1.Id; id <= PieceType.Token3.Id; id++)
        {
            positions.AddRange(board.BoardLogic.PositionsCache.GetPiecePositionsByType(id));
        }
            
        foreach (var position in positions)
        {
            board.ActionExecutor.AddAction(new CollapsePieceToAction
            {
                To = position,
                Positions = new List<BoardPosition> {position},
                AnimationResourceSearch = piece => AnimationOverrideDataService.Current.FindAnimation(piece, def => def.OnDestroyFromBoard)
            });
        }
    }

    private void RemoveCurrency()
    {
        ProfileService.Current.GetStorageItem(Currency.EventStep.Name).Amount = 0;
        ProfileService.Current.GetStorageItem(Currency.Token.Name).Amount = 0;
    }
}