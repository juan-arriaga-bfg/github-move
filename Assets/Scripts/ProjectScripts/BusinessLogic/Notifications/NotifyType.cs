using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class NotifyType
{
    private static List<NotifyType> notifyTypes = new List<NotifyType>();
    public static List<NotifyType> NotifyTypes => notifyTypes;
    
    static NotifyType()
    {
#if DEBUG
        var t = typeof(NotifyType);
        var fieldInfos = t.GetFields(BindingFlags.Public | BindingFlags.Static);
        
        for (int i = 0; i < fieldInfos.Length; i++)
        {
            var fieldInfo = fieldInfos[i];
            if (fieldInfo.FieldType != t) continue;
            
            var fieldValue = (NotifyType)fieldInfo.GetValue(null);
            
            notifyTypes.Add(fieldValue);
        }
#endif
    }
    
    /// <summary>
    /// Format 00_11 - where 00 notification category, 11 is index in category
    /// </summary>
    public int Id;
    public string TitleKey;
    public string MessageKey;
    public bool Override;
    public Func<DateTime, DateTime> TimeCorrector;
    public Func<List<Notifier>, List<Notifier>> NotifySelector;

#region Refresh

    public static readonly NotifyType MonumentRefreshComplete = new NotifyType
    {
        Id = 00_00, 
        TitleKey = "notifications.restore.monument.complete.title", 
        MessageKey = "notifications.restore.monument.complete.message",
        Override = true,
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
    
    public static readonly NotifyType MarketRefreshComplete = new NotifyType
    {
        Id = 00_01,
        TitleKey = "notifications.restore.market.complete.title",
        MessageKey = "notifications.restore.market.complete.message",
        Override = true,
        NotifySelector = notifiers =>
        {
            var resultNotifiers = new List<Notifier>();

            if (GameDataService.Current.TutorialDataManager.CheckMarket() == false)
            {
                return resultNotifiers;
            }

            return notifiers;
        },
        TimeCorrector = null
    };
    
    public static readonly NotifyType EnergyRefreshComplete = new NotifyType
    {
        Id = 00_02,
        TitleKey = "notifications.restore.energy.complete.title",
        MessageKey = "notifications.restore.energy.complete.message",
        Override = true,
        NotifySelector = null,
        TimeCorrector = notifyDate =>
        {
            var currentEnergy = ProfileService.Current.GetStorageItem(Currency.Energy.Name).Amount;
            var maxEnergy = ProfileService.Current.GetStorageItem(Currency.EnergyLimit.Name).Amount;
            var enrgyRefillDelay = GameDataService.Current.ConstantsManager.EnergyRefillDelay;
            return notifyDate.AddSeconds(enrgyRefillDelay * (maxEnergy - currentEnergy - 1));
        }
    };
    
    public static readonly NotifyType FreeEnergyRefreshComplete = new NotifyType
    {
        Id = 00_03, 
        TitleKey = "notifications.restore.free.energy.complete.title", 
        MessageKey = "notifications.restore.free.energy.complete.message",
        Override = true,
        NotifySelector = null,
    };

#endregion

#region Use worker

    public static readonly NotifyType MonumentBuildComplete = new NotifyType
    {
        Id = 01_00, 
        TitleKey = "notifications.build.monument.complete.title", 
        MessageKey = "notifications.build.monument.complete.message",
        Override = true,
        NotifySelector = null,
        TimeCorrector = null
    };

    public static readonly NotifyType BuildPieceComplete = new NotifyType
    {
        Id = 01_01,
        TitleKey = "notifications.build.piece.complete.title", 
        MessageKey = "notifications.build.piece.complete.message",
        Override = false,
        NotifySelector = null,
        TimeCorrector = null
    };
    
    public static readonly NotifyType BuildMineComplete = new NotifyType
    {
        Id = 01_02,
        TitleKey = "notifications.build.mine.complete.title", 
        MessageKey = "notifications.build.mine.complete.message",
        Override = false,
        NotifySelector = null,
        TimeCorrector = null
    };
    
    public static readonly NotifyType RemoveObstacleComplete = new NotifyType
    {
        Id = 01_03,
        TitleKey = "notifications.remove.obstacle.complete.title", 
        MessageKey = "notifications.remove.obstacle.complete.message",
        Override = false,
        NotifySelector = notifiers =>
        {
            var resultNotifiers = new List<Notifier>();
            foreach (var notifier in notifiers)
            {
                if (notifier.Timer.CompleteTime - notifier.Timer.StartTime > TimeSpan.FromSeconds(WorkerCurrencyLogicComponent.MinDelay))
                {
                    resultNotifiers.Add(notifier);
                }
            }
            return resultNotifiers;
        },
        TimeCorrector = null
    };
    
#endregion
    
#region Timeout soon
    
    public static readonly NotifyType DailyTimeout = new NotifyType
    {
        Id = 02_01, 
        TitleKey = "notifications.timeout.daily.objectives.title", 
        MessageKey = "notifications.timeout.daily.objectives.message",
        Override = true,
        NotifySelector = notifiers =>
        {
            var resultNotifiers = new List<Notifier>();
                
            var dailyQuestEntity = GameDataService.Current.QuestsManager.DailyQuest;

            if (dailyQuestEntity == null)
            {
                return resultNotifiers;
            }
            
            if (dailyQuestEntity.IsAllTasksClaimed(false) ||
                dailyQuestEntity.ActiveTasks.All(task => task.IsCompletedOrClaimed() == false))
                return resultNotifiers;
                
            foreach (var notifier in notifiers)
            {
                var leftTime = notifier.Timer.CompleteTime.GetTimeLeft(notifier.Timer.UseUTC);
                var localCorrect = DailyTimeout.TimeCorrector(DateTime.Now + leftTime);
                var globalCorrect = LocalNotificationsService.Current.CorrectTime(localCorrect);
                if (globalCorrect.Day == DateTime.UtcNow.Day)
                    resultNotifiers.Add(notifier);
            }
    
            return resultNotifiers;
        },
        TimeCorrector = notifyDate => notifyDate.Subtract(new TimeSpan(3, 30, 0))
    };
    
    public static readonly NotifyType FreeEnergyTimeout = new NotifyType
    {
        Id = 02_02, 
        TitleKey = "notifications.timeout.free.energy.title", 
        MessageKey = "notifications.timeout.free.energy.message",
        Override = true,
        NotifySelector = null,
        TimeCorrector = notifyDate => notifyDate.Subtract(new TimeSpan(0, 0, 15, 0))
    };

#endregion
    
#region Special

    public static readonly NotifyType ComeBackToGame = new NotifyType
    {
        Id = 99_01, 
        TitleKey = "notifications.come.back.title", 
        MessageKey = "notifications.come.back.message",
        Override = true,
        NotifySelector = null,
        TimeCorrector = notifyDate => notifyDate.Add(new TimeSpan(5, 0, 0, 0))
    };
    
    public static readonly NotifyType OrderComplete = new NotifyType
    {
        Id = 99_02,
        TitleKey = "notifications.order.complete.title", 
        MessageKey = "notifications.order.complete.message",
        Override = false,
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
    
#endregion
    
}