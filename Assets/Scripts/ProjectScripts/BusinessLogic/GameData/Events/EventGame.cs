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
    Start,
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
    public bool IsCompleteStep => Step == Steps.Count;
    
    private EventGamesLogicComponent context;
    
    public readonly TimerComponent TimeController = new TimerComponent();

    public DateTime StartTime { get; private set; }
    public DateTime IntroTime { get; private set; }
    public DateTime EndTime { get; private set; }
    
    public long StartTimeLong => StartTime.ConvertToUnixTime();
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        context = entity as EventGamesLogicComponent;
        RegisterComponent(TimeController);
    }
    
    public void InitData(long start, long end, long intro)
    {
        StartTime = DateTimeExtension.UnixTimeToDateTime(start);
        IntroTime = DateTimeExtension.UnixTimeToDateTime(intro);
        EndTime = DateTimeExtension.UnixTimeToDateTime(end);
    }

    public void InitData(DateTime start, DateTime end, int intro)
    {
        StartTime = start;
        IntroTime = StartTime.AddHours(intro);
        EndTime = end;
    }

    public void UpdateData(DateTime end, DateTime intro)
    {
        IntroTime = intro;
        EndTime = end;
    }

    public void Init()
    {
        switch (State)
        {
            case EventGameState.Start:
                TimeController.Delay = (int) (StartTime - SecuredTimeService.Current.UtcNow).TotalSeconds;
                TimeController.OnComplete = Preview;
                TimeController.Start();
                break;
            case EventGameState.Preview:
                Preview();
                break;
            case EventGameState.InProgress:
                Progress();
                break;
            case EventGameState.Complete:
                Complete();
                break;
            default:
                return;
        }
    }

    private void Preview()
    {
        var panel = ResourcePanelUtils.GetPanel(Steps[0].Prices[0].Currency) as UITokensPanelViewController;

        if (panel != null) panel.OnStartEventGame(EventType);
        
        ResourcePanelUtils.TogglePanel(Steps[0].Prices[0].Currency, true, true);
        
        State = EventGameState.Preview;
        TimeController.Delay = (int) (IntroTime - StartTime).TotalSeconds;
        TimeController.OnComplete = Progress;
        TimeController.Start(StartTime);
        
        if (IntroTime.ConvertToUnixTime() > SecuredTimeService.Current.UtcNow.ConvertToUnixTime()) OpenWindow(false);
    }

    private void Progress()
    {
        State = EventGameState.InProgress;
        TimeController.Delay = (int) (EndTime - StartTime).TotalSeconds;
        TimeController.OnComplete = Complete;
        TimeController.Start(StartTime);
        context.OnStart?.Invoke(EventType);
        
        if (EndTime.ConvertToUnixTime() > SecuredTimeService.Current.UtcNow.ConvertToUnixTime()) OpenWindow(false);
    }

    public void Complete()
    {
        TimeController.OnComplete = null;
        TimeController.Stop();
        State = EventGameState.Complete;
        OpenWindow(false);
    }

    public void OpenWindow(bool isFast)
    {
        ProfileService.Current.QueueComponent.RemoveAction($"{EventType.ToString()}_Window");

        void open ()
        {
            if (State == EventGameState.Preview)
            {
                var model = UIService.Get.GetCachedModel<UIEventPreviewWindowModel>(UIWindowType.EventPreviewWindow);

                model.Countdown = TimeController;
            
                UIService.Get.ShowWindow(UIWindowType.EventPreviewWindow);
                return;
            }
            
            UIService.Get.ShowWindow(UIWindowType.EventWindow);
        }

        if (isFast)
        {
            open();
            return;
        }
        
        DefaultSafeQueueBuilder.BuildAndRun($"{EventType.ToString()}_Window", true, open);
    }
    
    public void Finish()
    {
        State = EventGameState.Claimed;
        ResourcePanelUtils.TogglePanel(Steps[0].Prices[0].Currency, false, true);
        RemovePieces();
        context.OnStop?.Invoke(EventType);

        if (Step == Steps.Count)
        {
            RemoveCurrency();
            return;
        }
        
        var stepDef = Steps[Step];
        var model = UIService.Get.GetCachedModel<UIEventAlmostWindowModel>(UIWindowType.EventAlmostWindow);
        
        model.Step = Step + 1;
        model.Price = null;
        model.Rewards = new List<CurrencyPair>();
        
        RemoveCurrency();

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