using System.Collections.Generic;
using UnityEngine;

public class DailyRewardDataManager : ECSEntity, IDataManager, IDataLoader<List<DailyRewardDef>>
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    public List<DailyRewardDef> Defs;

    private const int TIMER_DELAY = 24 * 60 * 60;
    
    public TimerComponent Timer { get; private set; }
    
    public long TimerStartTime { get; private set; }

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
        TimerStartTime = save.TimerStart;
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