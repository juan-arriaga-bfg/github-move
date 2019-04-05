using System;
using System.Collections.Generic;
using UnityEngine;

public class DailyRewardDataManager : ECSEntity, IDataManager, IDataLoader<List<DailyRewardDef>>
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    public List<DailyRewardDef> Defs;

    private const int TIMER_DELAY = 24 * 60 * 60;
    
    public TimerComponent Timer { get; private set; }
    
    public DateTime TimerStartTime { get; private set; }


    public int Day { get; private set; }

    public int DaysCount => Defs.Count;
    
    private ECSEntity context;
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        context = entity;
        
        Reload();
    }

    public override void OnUnRegisterEntity(ECSEntity entity)
    {
        context = null;
    }
	
    public void Reload()
    {
        StopTimer();
        
        Defs = new List<DailyRewardDef>();
        LoadData(new ResourceConfigDataMapper<List<DailyRewardDef>>("configs/daily.data", NSConfigsSettings.Instance.IsUseEncryption));
        
        var save = ((GameDataManager)context).UserProfile.GetComponent<DailyRewardSaveComponent>(DailyRewardSaveComponent.ComponentGuid);
        Day = save.Day;
        if (Day - 1 > Defs.Count)
        {
            IW.Logger.LogWarning($"[DailyRewardDataManager] => Reload: Day = {Day} but we have only {Defs.Count} Defs. Sequence reseted!");
            Day = 0;
        }
        
        TimerStartTime = DateTimeExtension.UnixTimeToDateTime(save.TimerStart);
    }

    public void StopTimer()
    {
        if (Timer != null)
        {
            Timer.OnComplete -= OnCompleteTimer;
            Timer.Stop();
            Timer = null;
        }
    }

    public void StartTimer()
    {
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

    private void OnCompleteTimer()
    {
        string userGroup = GameDataService.Current.AbTestManager.Tests[AbTestName.DAILY_REWARD].UserGroup;

        var model = UIService.Get.GetCachedModel<UIDailyRewardWindowModel>(UIWindowType.DailyRewardWindow);
        model.Defs = Defs;
        model.Day = GameDataService.Current.DailyRewardManager.Day;

        switch (userGroup)
        {
            // Reset sequence if player skipped a day
            case "a":
                var currentTime = SecuredTimeService.Current.Now;
                var timerStartTime = TimerStartTime;
                var delta = Mathf.Abs((float)(currentTime - timerStartTime).TotalDays);
                if (delta > 1)
                {
                    IW.Logger.LogWarning($"[DailyRewardDataManager] => OnCompleteTimer: Skip dialog delta == {delta}. currentTime = {currentTime}, timerStartTime = {timerStartTime}");
                    ResetSequence();
                    StartTimer();
                    break;
                }
                
                UIService.Get.ShowWindow(UIWindowType.DailyRewardWindow);
                NextDay();
                break;
            
            // Just continue sequence, even if player skipped a day
            case "b":
                UIService.Get.ShowWindow(UIWindowType.DailyRewardWindow);
                NextDay();
                break;
            
            // No daily reward at all
            case "c":
                IW.Logger.LogWarning($"[DailyRewardDataManager] => OnCompleteTimer: Skip dialog because user belongs to test group 'c'");
                break;
        }
        
        UIService.Get.ShowWindow(UIWindowType.DailyRewardWindow);
    }

    private void NextDay()
    {
        Day++;
        
        if (Day > Defs.Count - 1)
        {
            ResetSequence();
        }
    }

    private void ResetSequence()
    {
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
}