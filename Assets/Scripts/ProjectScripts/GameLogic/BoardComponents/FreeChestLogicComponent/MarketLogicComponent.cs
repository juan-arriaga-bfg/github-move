using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MarketLogicComponent : ECSEntity
{
    const int FREE_ENEGRY_SLOT_INDEX = 0;
    
    private struct TimeRange
    {
        public long From {get; private set;}
        public long To   {get; private set;}

        public TimeRange(long from, long to)
        {
            From = from;
            To = to;
        }
        
        public bool Contains(long time)
        {
            return time >= From && time < To;
        }
    }
    
    public enum EnergySlotState
    {
        WaitForReset,
        WaitForClaim
    }
    
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	public override int Guid => ComponentGuid;

	public TimerComponent ResetMarketTimer { get; } = new TimerComponent();
	public TimerComponent ResetEnergyTimer { get; } = new TimerComponent();
	public TimerComponent ClaimEnergyTimer { get; } = new TimerComponent();
	public TimerComponent OfferTimer { get; } = new TimerComponent();
	
	public TimerComponent FreeEnergyServiceTimer { get; } = new TimerComponent {Tag = "FreeEnergyServiceTimer"};
	public TimerComponent FreeEnergyExpireServiceTimer { get; } = new TimerComponent {Tag = "FreeEnergyExpireServiceTimer"};

    public DateTime FreeEnergyClaimTime { get; private set; } = UnixTimeHelper.UnixTimestampToDateTime(0);

    public bool FirstFreeEnergyClaimed { get; private set; }

	public int OfferIndex;
	public ShopDef Offer;

    private bool isFreeEnergyNotifierRegistered;
    
	public override void OnRegisterEntity(ECSEntity entity)
	{
		RegisterComponent(ResetMarketTimer, true);
		RegisterComponent(ResetEnergyTimer, true);
		RegisterComponent(ClaimEnergyTimer, true);
		RegisterComponent(OfferTimer, true);
		RegisterComponent(FreeEnergyServiceTimer, true);
        RegisterComponent(FreeEnergyExpireServiceTimer, true);
		
		LocalNotificationsService.Current.RegisterNotifier(new Notifier(ResetMarketTimer, NotifyType.MarketRefreshComplete));
	
        InitResetMarketTimer();

        InitEnergyTimers();
	}

    private void InitEnergyTimers()
    {
        var save = ProfileService.Current.GetComponent<MarketSaveComponent>(MarketSaveComponent.ComponentGuid);
        if (save != null)
        {
            FreeEnergyClaimTime = UnixTimeHelper.UnixTimestampToDateTime(save.FreeEnergyClaimTime);
            FirstFreeEnergyClaimed = save.FirstEnergyClaimed;
        }

        ResetEnergyTimer.OnComplete += UpdateEnergyTimers;

        ClaimEnergyTimer.OnComplete += UpdateEnergyTimers;
        
        UpdateEnergyTimers();
    }

    private void InitResetMarketTimer()
    {
        ResetMarketTimer.Delay = GameDataService.Current.ConstantsManager.MarketUpdateDelay;
        
        ResetMarketTimer.OnComplete += () =>
        {
            GameDataService.Current.MarketManager.UpdateSlots(true);
            ResetMarketTimer.Start();
        };

        var save = ProfileService.Current.GetComponent<MarketSaveComponent>(MarketSaveComponent.ComponentGuid);

        if (save != null && string.IsNullOrEmpty(save.ResetMarketStartTime) == false)
        {
            ResetMarketTimer.Start(long.Parse(save.ResetMarketStartTime));
        }
        else
        {
            ResetMarketTimer.Start();
        }
    }

    private void UpdateEnergyTimers()
    {
        if (!FirstFreeEnergyClaimed)
        {
            ResetEnergyTimer.Stop();
            ClaimEnergyTimer.Delay = int.MaxValue;
            ClaimEnergyTimer.Start();
            
            FreeEnergyServiceTimer.Delay = int.MaxValue;
            FreeEnergyExpireServiceTimer.Delay = int.MaxValue;
            
            FreeEnergyServiceTimer.Start();
            FreeEnergyExpireServiceTimer.Start();
        }
        else
        {
            EnergySlotState state = CheckEnergySlot(out int resetDelay, out int claimDelay);
            UpdateServiceTimers(resetDelay, claimDelay);
            
            switch (state)
            {
                case EnergySlotState.WaitForReset:
                    IW.Logger.Log($"[MarketLogicComponent] => UpdateEnergyTimers: WaitForReset");

                    ResetEnergyTimer.Delay = resetDelay;
                    ClaimEnergyTimer.Stop();
                    ResetEnergyTimer.Start();
                    break;

                case EnergySlotState.WaitForClaim:
                    IW.Logger.Log($"[MarketLogicComponent] => UpdateEnergyTimers: WaitForClaim");

                    ClaimEnergyTimer.Delay = claimDelay;
                    ResetEnergyTimer.Stop();
                    ClaimEnergyTimer.Start();
                    break;
            }
        }

        RegisterNotifiers();
    }

    private void RegisterNotifiers()
    {
        if (!isFreeEnergyNotifierRegistered && FirstFreeEnergyClaimed)
        {
            isFreeEnergyNotifierRegistered = true;
            LocalNotificationsService.Current.RegisterNotifier(new Notifier(FreeEnergyServiceTimer, NotifyType.FreeEnergyRefreshComplete));
            LocalNotificationsService.Current.RegisterNotifier(new Notifier(FreeEnergyExpireServiceTimer, NotifyType.FreeEnergyTimeout));
        }
    }

    private void UpdateServiceTimers(int resetDelay, int claimDelay)
    {
        FreeEnergyServiceTimer.Delay = resetDelay;
            
        FreeEnergyExpireServiceTimer.Delay = claimDelay;
        if (claimDelay == -1)
        {
            int delayForClaim = GameDataService.Current.ConstantsManager.DelayToClaimFreeEnergy;
            FreeEnergyExpireServiceTimer.Delay = resetDelay + delayForClaim;
        }
        
        FreeEnergyServiceTimer.Start();
        FreeEnergyExpireServiceTimer.Start();
    }
    
    public void FreeEnergyClaim()
    {
        FirstFreeEnergyClaimed = true;
        
        FreeEnergyClaimTime = SecuredTimeService.Current.Now;
        
        IW.Logger.Log($"[MarketLogicComponent] => FreeEnergyClaim: at {FreeEnergyClaimTime}");
        
        CheckEnergySlot(out int resetDelay, out int claimDelay);
        
        ClaimEnergyTimer.Stop();
        
        ResetEnergyTimer.Delay = resetDelay;
        ResetEnergyTimer.Start();
        
        UpdateServiceTimers(resetDelay, claimDelay);
        
        RegisterNotifiers();
    }

    private int GetSecondsElapsedFromStartOfDay()
    {
        DateTime currentTime = SecuredTimeService.Current.Now;
        DateTime todayDayStart = currentTime.TruncDateTimeToDays();
        TimeSpan elapsedTimeFromDayStart = currentTime - todayDayStart;
        int elapsedSeconds = (int)elapsedTimeFromDayStart.TotalSeconds;

        return elapsedSeconds;
    }

    private EnergySlotState CheckEnergySlot(out int resetDelay, out int claimDelay)
    {
        const int SECONDS_IN_DAY = 24 * 60 * 60;

        DateTime currentTime = SecuredTimeService.Current.Now;
        DateTime todayDayStart = currentTime.TruncDateTimeToDays();
        int elapsedSeconds = GetSecondsElapsedFromStartOfDay();
        int delayForClaim = GameDataService.Current.ConstantsManager.DelayToClaimFreeEnergy;
        
        int[] delays = GameDataService.Current.ShopManager.Defs[Currency.Energy.Name][FREE_ENEGRY_SLOT_INDEX].Delays;

#if DEBUG
        StringBuilder sb = new StringBuilder("[MarketLogicComponent] => CheckEnergySlot: Input\n");
        sb.AppendLine($"currentTime   : {currentTime}");
        sb.AppendLine($"todayDayStart : {todayDayStart}");
        sb.AppendLine($"elapsedSeconds: {elapsedSeconds}s");
        sb.AppendLine($"delayForClaim : {delayForClaim}s");

        sb.AppendLine("Reset on: ");
        for (int day = 0; day < 3; day++)
        {
            foreach (var delay in delays)
            {
                TimeSpan ts = TimeSpan.FromSeconds(delay + SECONDS_IN_DAY * day);
                sb.AppendLine($"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00} ({ts.TotalSeconds}s)");
            }

            sb.AppendLine();
        }

        IW.Logger.Log(sb.ToString());
#endif
        
        long claimOccuredAfterSecondsFromDayStart = (long)(FreeEnergyClaimTime - todayDayStart).TotalSeconds;
        IW.Logger.Log($"[MarketLogicComponent] => CheckEnergySlot: todayDayStart: {todayDayStart}, FreeEnergyClaimTime: {FreeEnergyClaimTime},  elapsedTodayAfterClaim: {claimOccuredAfterSecondsFromDayStart}");

        
        EnergySlotState ret = EnergySlotState.WaitForReset;
        
        // // case whe we should wait for the next day
        // var secondsToEndOfDay = SECONDS_IN_DAY - elapsedSeconds;
        // resetDelay = secondsToEndOfDay + delays[0];

        resetDelay = -1;
        
        // Check delays
        for (int day = 0; day < 3; day++)
        {
            foreach (var delay in delays)
            {
                int delayToCheck = delay + SECONDS_IN_DAY * day;
                
                if (delayToCheck < claimOccuredAfterSecondsFromDayStart)
                {
                    IW.Logger.Log($"[MarketLogicComponent] => CheckEnergySlot: Select Reset Delay: Current delay: {delayToCheck} < last claim time ({claimOccuredAfterSecondsFromDayStart})");
                    continue;
                }
                
                if (elapsedSeconds < delayToCheck)
                {
                    resetDelay = delayToCheck - elapsedSeconds;
                    IW.Logger.Log($"[MarketLogicComponent] => CheckEnergySlot: Select Reset Delay: Selected: {delayToCheck}, elapsedSeconds: {elapsedSeconds}, calculated resetDelay: {resetDelay}");
                    break;
                }

                IW.Logger.Log($"[MarketLogicComponent] => CheckEnergySlot: Select Reset Delay: Skip {delayToCheck}, elapsedSeconds: {elapsedSeconds}");
            }
            
            if (resetDelay != -1)
            {
                break;
            }
        }

        if (resetDelay == -1)
        {
            IW.Logger.LogError($"[MarketLogicComponent] => CheckEnergySlot: resetDelay calc error! Check the code!");
        }

        List<TimeRange> claimRanges = new List<TimeRange>();
        foreach (var delay in delays)
        {
            claimRanges.Add(new TimeRange(delay, delay + delayForClaim));
        }

        claimDelay = -1;
     
        foreach (var claimRange in claimRanges)
        {
            if (claimRange.From < claimOccuredAfterSecondsFromDayStart)
            {
                IW.Logger.Log($"[MarketLogicComponent] => CheckEnergySlot: Select Claim Range: Current range start ({claimRange.From}) < last claim time ({claimOccuredAfterSecondsFromDayStart})");
                continue;
            }

            if (claimRange.Contains(elapsedSeconds))
            {
                if (claimRange.Contains(claimOccuredAfterSecondsFromDayStart))
                {
                    IW.Logger.Log($"[MarketLogicComponent] => CheckEnergySlot: Select Claim Range: Current range ({claimRange.From}-{claimRange.To}) already claimed at {claimOccuredAfterSecondsFromDayStart}");
                    continue;
                }
                
                IW.Logger.Log($"[MarketLogicComponent] => CheckEnergySlot: Select Claim Range: Selected range: {claimRange.From}-{claimRange.To}");
                
                claimDelay = (int)claimRange.To - elapsedSeconds;

                ret = EnergySlotState.WaitForClaim;
                break;
            }
        }

        IW.Logger.Log($"[MarketLogicComponent] => CheckEnergySlot: returns: state: {ret}, resetDelay: {resetDelay}, claimDelay: {claimDelay}");
        
        return ret;
    }

    public override void OnUnRegisterEntity(ECSEntity entity)
    {
        ResetMarketTimer.OnComplete = null;
        ResetEnergyTimer.OnComplete = null;
        ClaimEnergyTimer.OnComplete = null;
        FreeEnergyServiceTimer.OnTimeChanged = null;
        
        LocalNotificationsService.Current.UnRegisterNotifier(ResetMarketTimer);
        
        if (isFreeEnergyNotifierRegistered)
        {
            LocalNotificationsService.Current.UnRegisterNotifier(FreeEnergyServiceTimer);
            LocalNotificationsService.Current.UnRegisterNotifier(FreeEnergyExpireServiceTimer);
        }
        
        base.OnUnRegisterEntity(entity);
    }

    public void CompleteOffer()
    {
	    if (ProfileService.Current.GetStorageItem(Currency.Offer.Name).Amount >= OfferIndex) return;
	    
	    CurrencyHelper.Purchase(Currency.Offer.Name, 1);
    }
}