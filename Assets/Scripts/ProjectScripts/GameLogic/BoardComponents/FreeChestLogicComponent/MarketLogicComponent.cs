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
	
	public TimerComponent FreeEnergyLocalNotificationTimer { get; } = new TimerComponent();

    public DateTime FreeEnergyClaimTime { get; private set; } = UnixTimeHelper.UnixTimestampToDateTime(0);

    public bool FirstFreeEnergyClaimed { get; private set; } 
    
	public int OfferIndex;
	public ShopDef Offer;

	public override void OnRegisterEntity(ECSEntity entity)
	{
		RegisterComponent(ResetMarketTimer, true);
		RegisterComponent(ResetEnergyTimer, true);
		RegisterComponent(ClaimEnergyTimer, true);
		RegisterComponent(OfferTimer, true);
		
		LocalNotificationsService.Current.RegisterNotifier(new Notifier(ResetMarketTimer, NotifyType.MarketRefresh));
		LocalNotificationsService.Current.RegisterNotifier(new Notifier(FreeEnergyLocalNotificationTimer, NotifyType.FreeEnergyRefill));
				
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

        ResetEnergyTimer.OnComplete += () =>
        {
            UpdateEnergyTimers();
        };

        ClaimEnergyTimer.OnComplete += () =>
        {
            UpdateEnergyTimers();
        };
        
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
            return;
        }
        
        EnergySlotState state = CheckEnergySlot(out int resetDelay, out int claimDelay);

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
                
                ResetEnergyTimer.Stop();
                ClaimEnergyTimer.Delay = claimDelay;
                ClaimEnergyTimer.Start();
                break;
        }

        FreeEnergyLocalNotificationTimer.Delay = resetDelay;
        FreeEnergyLocalNotificationTimer.Start();
    }
    
    public void FreeEnergyClaim()
    {
        FirstFreeEnergyClaimed = true;
        
        FreeEnergyClaimTime = SecuredTimeService.Current.Now;
        
        CheckEnergySlot(out int resetDelay, out int claimDelay);
        
        ClaimEnergyTimer.Stop();
        
        ResetEnergyTimer.Delay = resetDelay;
        ResetEnergyTimer.Start();
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
        foreach (var delay in delays)
        {
            TimeSpan ts = TimeSpan.FromSeconds(delay);
            sb.AppendLine($"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00} ({ts.TotalSeconds}s)");
        }
        
        IW.Logger.Log(sb.ToString());
#endif
        EnergySlotState ret = EnergySlotState.WaitForReset;
        
        // case whe we should wait for the next day
        var secondsToEndOfDay = SECONDS_IN_DAY - elapsedSeconds;
        resetDelay = secondsToEndOfDay + delays[0];

        // Check delays for today
        foreach (var delay in delays)
        {
            if (elapsedSeconds < delay)
            {
                resetDelay = delay - elapsedSeconds;
                break;
            } 
        }

        List<TimeRange> claimRanges = new List<TimeRange>();
        foreach (var delay in delays)
        {
            claimRanges.Add(new TimeRange(delay, delay + delayForClaim));
        }

        claimDelay = -1;

        long claimOccuredAfterSecondsFromDayStart = (long)(FreeEnergyClaimTime - todayDayStart).TotalSeconds;

        IW.Logger.Log($"[MarketLogicComponent] => CheckEnergySlot: todayDayStart: {todayDayStart}, FreeEnergyClaimTime: {FreeEnergyClaimTime},  elapsedTodayAfterClaim: {claimOccuredAfterSecondsFromDayStart}");
        
        foreach (var claimRange in claimRanges)
        {
            // Time to claim!
            if (claimRange.Contains(elapsedSeconds))
            {
                if (claimRange.Contains(claimOccuredAfterSecondsFromDayStart))
                {
                    IW.Logger.Log($"[MarketLogicComponent] => CheckEnergySlot: Current range already claimed");
                    continue;
                }
                
                claimDelay = (int)claimRange.To - elapsedSeconds;

                ret = EnergySlotState.WaitForClaim;
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
        
        base.OnUnRegisterEntity(entity);
    }

    public void CompleteOffer()
    {
	    if (ProfileService.Current.GetStorageItem(Currency.Offer.Name).Amount >= OfferIndex) return;
	    
	    CurrencyHelper.Purchase(Currency.Offer.Name, 1);
    }
}