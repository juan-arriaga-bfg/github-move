using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

public class NotifyType
{
    public int Id;
    public string TitleKey;
    public string MessageKey;
    public bool Override;
    public Func<DateTime, DateTime> TimeCorrector;
    public Func<List<Notifier>, List<Notifier>> NotifySelector;
    
    public static NotifyType MonumentRefresh = new NotifyType
    {
        Id = 0, 
        TitleKey = "notifications.monument.refresh.title", 
        MessageKey = "notifications.monument.refresh.message", 
        NotifySelector = (notifiers) =>
        {
            if (notifiers.Count == 0)
                return notifiers;
            var minimalTime = notifiers.Min(elem => elem.Timer.CompleteTime);
            var resultNotifier = notifiers.First(elem => elem.Timer.CompleteTime == minimalTime);
            return new List<Notifier>() { resultNotifier };
        },
        TimeCorrector = null
    };
    
    public static NotifyType MonumentBuild = new NotifyType
    {
        Id = 1, 
        TitleKey = "notifications.monument.build.title", 
        MessageKey = "notifications.monument.build.message",  
        NotifySelector = null,
        TimeCorrector = null
    };
    
    public static NotifyType DailyTimeout = new NotifyType
    {
        Id = 2, 
        TitleKey = "notifications.daily.timeout.title", 
        MessageKey = "notifications.daily.timeout.message",  
        NotifySelector = notifiers =>
        {
            var resultNotifiers = new List<Notifier>();
            foreach (var notifier in notifiers)
            {
                if(LocalNotificationsService.Current.CorrectTime(DailyTimeout.TimeCorrector(notifier.Timer.CompleteTime)).Day == DateTime.Now.Day)
                    resultNotifiers.Add(notifier);
            }

            return resultNotifiers;
        },
        TimeCorrector = notifyDate => notifyDate.Subtract(new TimeSpan(3, 30, 0))
    };

    public static NotifyType MarketRefresh = new NotifyType
    {
        Id = 3,
        TitleKey = "notifications.market.refresh.title",
        MessageKey = "notifications.market.refresh.message",
        NotifySelector = null,
        TimeCorrector = null
    };
    
    public static NotifyType EnergyRefresh = new NotifyType
    {
        Id = 4,
        TitleKey = "notifications.energy.refresh.title",
        MessageKey = "notifications.energy.refresh.message",
        NotifySelector = null,
        TimeCorrector = notifyDate =>
        {
            var currentEnergy = ProfileService.Current.GetStorageItem(Currency.Energy.Name).Amount;
            var maxEnergy = ProfileService.Current.GetStorageItem(Currency.EnergyLimit.Name).Amount;
            var enrgyRefillDelay = GameDataService.Current.ConstantsManager.EnergyRefillDelay;
            return notifyDate.AddSeconds(enrgyRefillDelay * (maxEnergy - currentEnergy - 1));
        }
    };
}