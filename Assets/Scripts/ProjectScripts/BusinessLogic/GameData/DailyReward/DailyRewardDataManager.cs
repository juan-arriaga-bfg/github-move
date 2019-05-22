using System;
using System.Collections.Generic;
using BfgAnalytics;
using UnityEngine;

public class DailyRewardDataManager : ECSEntity, IDataManager, IDataLoader<List<DailyRewardDef>>
{
    public const string QUEST_ID = "66_CreatePiece_NPC_B3";

    private const int MIN_DELAY_BETWEEN_REWARDS = 60 * 60;
    
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    public List<DailyRewardDef> Defs;

    private const int TIMER_DELAY = 24 * 60 * 60;
    
    public TimerComponent Timer { get; private set; }
    
    public DateTime TimerStartTime { get; private set; }

    public int Day { get; private set; } = -1;

    public int DaysCount => Defs.Count;

    private GameDataManager context;

    public bool IsActivated { get; private set; }
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        context = (GameDataManager)entity;
        
        Reload();
    }

    public override void OnUnRegisterEntity(ECSEntity entity)
    {
        context = null;
        
        Cleanup();
    }
	
    public void Reload()
    {
        Cleanup();
        
        Defs = new List<DailyRewardDef>();
        LoadData(new ResourceConfigDataMapper<List<DailyRewardDef>>("configs/daily.data", NSConfigsSettings.Instance.IsUseEncryption));
        
        var save = context.UserProfile.GetComponent<DailyRewardSaveComponent>(DailyRewardSaveComponent.ComponentGuid);
        if (save == null)
        {
            return;
        }
        
        Day = save.Data.Day;
        if (Day - 1 > Defs.Count)
        {
            IW.Logger.LogWarning($"[DailyRewardDataManager] => Reload: Day = {Day} but we have only {Defs.Count} Defs. Sequence will be reseted!");
            ResetSequence();
        }
        
        TimerStartTime = DateTimeExtension.UnixTimeToDateTime(save.Data.TimerStart);

        IsActivated = save.Data.IsActivated;
    }

    public void StopTimer()
    {
        if (Timer != null)
        {
            Timer.OnComplete -= OnCompleteTimer;
            Timer.Stop();
            UnRegisterComponent(Timer);
            Timer = null;
        }
    }

    public void StartTimer()
    {
        if (!IsActivated)
        {
            IW.Logger.Log($"[DailyRewardDataManager] => StartTimer: Skip by !IsActivated");

            var questManager = context.QuestsManager;
            if (questManager.IsQuestCompleted(QUEST_ID))
            {
                FirstRun();
            }
            else
            {
                questManager.OnQuestStateChanged += OnQuestStateChanged;
            }
            
            return;
        }
        
        StopTimer();

        IW.Logger.Log($"[DailyRewardDataManager] => StartTimer: Start: {TimerStartTime} End: {TimerStartTime.AddSeconds(TIMER_DELAY)}");
        
        Timer = new TimerComponent
        {
            UseUTC = false,
            Delay = TIMER_DELAY,
            Tag = "dailyReward"
        };
        
        Timer.OnComplete += OnCompleteTimer;

        RegisterComponent(Timer);
        Timer.Start(TimerStartTime);
    }

    private void OnQuestStateChanged(QuestEntity quest, TaskEntity task)
    {
        if (quest.Id == QUEST_ID && quest.IsClaimed())
        {
            FirstRun();
        }
    }

    private void Cleanup()
    {
        context.QuestsManager.OnQuestStateChanged -= OnQuestStateChanged;

        StopTimer();
    }

    private DateTime CalculateTimerStartTime()
    {
        var secureTime = SecuredTimeService.Current;
        var now = secureTime.Now;
        
        DateTime timerCompleteTime = Timer?.CompleteTime ?? DateTime.MinValue;
        
        if (now < timerCompleteTime)
        {
            IW.Logger.Log($"[DailyRewardDataManager] => CalculateTimerStartTime: Now ({now}) < Last timer complete time ({timerCompleteTime}). timerCompleteTime will be used as current.");
            now = timerCompleteTime;
        }
        
        var todayDayStart = now.TruncDateTimeToDays();

        long deltaSeconds = (long)(now.AddSeconds(MIN_DELAY_BETWEEN_REWARDS) - todayDayStart.AddSeconds(TIMER_DELAY)).TotalSeconds;
        if (deltaSeconds > 0)
        {
            IW.Logger.Log($"[DailyRewardDataManager] => CalculateTimerStartTime: Timer adjusted by {deltaSeconds}s to fit min delay ({MIN_DELAY_BETWEEN_REWARDS})");
            todayDayStart = todayDayStart.AddSeconds(deltaSeconds);
        }
        
        return todayDayStart;
    }

    private void ShowWindow()
    {
        IW.Logger.Log($"[DailyRewardDataManager] => ShowWindow: Added to queue");
        
        const string QUEUE_ACTION_ID = "DailyReward";
        DefaultSafeQueueBuilder.BuildAndRun(QUEUE_ACTION_ID, true, () =>
        {
            IW.Logger.Log($"[DailyRewardDataManager] => ShowWindow: Queue action exec!");
            
            var model = UIService.Get.GetCachedModel<UIDailyRewardWindowModel>(UIWindowType.DailyRewardWindow);
            model.Defs = Defs;
            model.Day = context.DailyRewardManager.Day;
        
            UIService.Get.ShowWindow(UIWindowType.DailyRewardWindow);
        });
    }
    
    private void OnCompleteTimer()
    {
        string userGroup = context.AbTestManager.Tests[AbTestName.DAILY_REWARD].UserGroup;

        // userGroup = "a";
        
        IW.Logger.Log($"[DailyRewardDataManager] => OnCompleteTimer: userGroup: {userGroup}");
        
        switch (userGroup)
        {
            // Reset sequence if player skipped a day
            case "a":
                var currentTime = SecuredTimeService.Current.Now;
                var timerStartTime = TimerStartTime;
                var delta = Mathf.Abs((float)(currentTime - timerStartTime).TotalDays);
                if (delta > 2)
                {
                    IW.Logger.Log($"[DailyRewardDataManager] => OnCompleteTimer (a): Reset to 1st day: currentTime: {currentTime}, timerStartTime: {timerStartTime}, delta: {delta}");
                    ResetSequence();
                }
                
                ShowWindow();
                break;
            
            // Just continue sequence, even if player skipped a day
            case "b":
                
                IW.Logger.Log($"[DailyRewardDataManager] => OnCompleteTimer (b)");
                
                ShowWindow();
                break;
            
            // No daily reward at all
            case "c":
                IW.Logger.LogWarning($"[DailyRewardDataManager] => OnCompleteTimer (c): Skip dialog because user belongs to test group 'c'");
                break;
        }
    }

    private void RestartTimer()
    {
        TimerStartTime = CalculateTimerStartTime();
        
        IW.Logger.Log($"[DailyRewardDataManager] => RestartTimer");

        StartTimer();
    }

    public void ClaimCurrentDay()
    {
        IW.Logger.Log($"[DailyRewardDataManager] => ConsumeCurrentDay");

        IsActivated = true;
        
        Analytics.SendDailyRewardClaim(Day + 1);
        
        NextDay();
    }
    
    private void NextDay()
    {
        IW.Logger.Log($"[DailyRewardDataManager] => NextDay");
        
        Day++;
        
        if (Day > Defs.Count - 1)
        {
            ResetSequence();
        }

        RestartTimer();
    }

    private void ResetSequence()
    {
        IW.Logger.Log($"[DailyRewardDataManager] => ResetSequence");
        
        Day = 0;
    }

    public void LoadData(IDataMapper<List<DailyRewardDef>> dataMapper)
    {
        dataMapper.LoadData((data, error) =>
        {
            if (string.IsNullOrEmpty(error))
            {
                Defs = data;
            }
            else
            {
                Debug.LogWarningFormat("[{0}]: config not loaded", GetType());
            }
        });
    }

    public void FirstRun()
    {
        IW.Logger.Log($"[DailyRewardDataManager] => Activate");

        Cleanup();
        ResetSequence();
        ShowWindow();
    }
}