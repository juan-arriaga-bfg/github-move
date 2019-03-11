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
    
    public static readonly NotifyType MonumentRefresh = new NotifyType
    {
        Id = 0, 
        TitleKey = "notifications.monument.restore.title", 
        MessageKey = "notifications.monument.restore.message", 
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

    public static readonly NotifyType MonumentBuild = new NotifyType
    {
        Id = 1, 
        TitleKey = "notifications.monument.build.title", 
        MessageKey = "notifications.monument.build.message",  
        NotifySelector = null,
        TimeCorrector = null
    };
    
    public static readonly NotifyType DailyTimeout = new NotifyType
    {
        Id = 2, 
        TitleKey = "notifications.daily.timeout.title", 
        MessageKey = "notifications.daily.timeout.message",  
        NotifySelector = notifiers =>
        {
            var resultNotifiers = new List<Notifier>();
            
            var dailyQuestEntity = GameDataService.Current.QuestsManager.DailyQuest;
            if (dailyQuestEntity.IsAllTasksClaimed(false) ||
                dailyQuestEntity.ActiveTasks.All(task => task.IsCompletedOrClaimed() == false))
                return resultNotifiers;
            
            foreach (var notifier in notifiers)
            {
                if(LocalNotificationsService.Current.CorrectTime(DailyTimeout.TimeCorrector(notifier.Timer.CompleteTime)).Day == DateTime.UtcNow.Day)
                    resultNotifiers.Add(notifier);
            }

            return resultNotifiers;
        },
        TimeCorrector = notifyDate => notifyDate.Subtract(new TimeSpan(3, 30, 0))
    };

    public static readonly NotifyType MarketRefresh = new NotifyType
    {
        Id = 3,
        TitleKey = "notifications.market.restore.title",
        MessageKey = "notifications.market.restore.message",
        NotifySelector = notifiers =>
        {
            var resultNotifiers = new List<Notifier>();
      
            var tutorialLogic = BoardService.Current.FirstBoard.TutorialLogic;
            if (tutorialLogic.CheckMarket() == false)
                return resultNotifiers;

            return notifiers;
        },
        TimeCorrector = null
    };
    
    public static readonly NotifyType EnergyRefresh = new NotifyType
    {
        Id = 4,
        TitleKey = "notifications.energy.restore.title",
        MessageKey = "notifications.energy.restore.message",
        NotifySelector = null,
        TimeCorrector = notifyDate =>
        {
            var currentEnergy = ProfileService.Current.GetStorageItem(Currency.Energy.Name).Amount;
            var maxEnergy = ProfileService.Current.GetStorageItem(Currency.EnergyLimit.Name).Amount;
            var enrgyRefillDelay = GameDataService.Current.ConstantsManager.EnergyRefillDelay;
            return notifyDate.AddSeconds(enrgyRefillDelay * (maxEnergy - currentEnergy - 1));
        }
    };
    
    public static readonly NotifyType ComeBackToGame = new NotifyType
    {
        Id = 5, 
        TitleKey = "notifications.come.back.title", 
        MessageKey = "notifications.come.back.message",  
        NotifySelector = null,
        TimeCorrector = notifyDate => notifyDate.Add(new TimeSpan(5, 0, 0, 0))
    };
    
    public static readonly NotifyType FreeEnergyRefill = new NotifyType
    {
        Id = 6, 
        TitleKey = "notifications.free.energy.title", 
        MessageKey = "notifications.free.energy.message",  
        NotifySelector = null,
    };
}