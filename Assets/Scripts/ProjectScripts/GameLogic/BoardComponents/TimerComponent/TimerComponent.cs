using System;
using System.Collections.Generic;
using BfgAnalytics;
using UnityEngine;

public class TimerComponent : IECSComponent, IECSSystem, ITimerComponent
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;

    public int Delay;
    
    public Action OnStart;
    
    /// <summary>
    /// Raised every frame. DO NOT use for updating labels to avoid heavy GC load.
    /// </summary>
    public Action OnExecute;
    
    /// <summary>
    /// Raised once per second.
    /// </summary>
    public Action OnTimeChanged;
    
    public Action OnComplete;
    
    public Action OnStop;
    
    public DateTime StartTime { get; set; }
    public DateTime CompleteTime { get; set; }
    
    public BoardTimerView View;

    public bool UseUTC { get; set; } = true;
    
    public long StartTimeLong => StartTime.ConvertToUnixTime(UseUTC);

    public bool IsStarted;
    public bool IsPaused;
    public bool IsCanceled;
    
    private CurrencyPair price = new CurrencyPair{Currency = Currency.Crystals.Name};

    public string Tag;

    private int lastProcessedSecond = -1;  
    
    public void OnRegisterEntity(ECSEntity entity)
    {
    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }
    
    public void Start()
    {
        // Start(UseUTC ? DateTime.UtcNow : DateTime.Now);
        var secureTime = SecuredTimeService.Current;
        Start(UseUTC ? secureTime.UtcNow : secureTime.Now);
    }

    public void Start(long start)
    {
        Start(DateTimeExtension.UnixTimeToDateTime(start, UseUTC));
    }
    
    public void Start(DateTime start)
    {
        StartTime = start;
        CompleteTime = StartTime.AddSeconds(Delay);
        
        IsStarted = true;
        IsPaused = false;
        
        OnStart?.Invoke();
    }
    
    public void Stop()
    {
        IsStarted = false;
        OnStop?.Invoke();
    }
    
    public void Execute()
    {
        OnExecute?.Invoke();

        var elapsedTime = StartTime.GetTime(UseUTC);
        int elapsedSeconds = (int) elapsedTime.TotalSeconds;

        if (lastProcessedSecond < 0 || lastProcessedSecond != elapsedSeconds)
        {
            lastProcessedSecond = elapsedSeconds;
            OnTimeChanged?.Invoke();
        }
        
        if (elapsedSeconds < Delay)
        {
            return;
        }

        IsStarted = false;

        OnComplete?.Invoke();
    }
    
    public bool IsExecuteable()
    {
        return IsPaused == false && IsStarted;
    }
    
    public object GetDependency()
    {
        return null;
    }
    
    public float GetProgress()
    {
        return (int)StartTime.GetTime(UseUTC).TotalSeconds / (float)Delay;
    }
    
    public void Complete()
    {
        Stop();
        OnComplete?.Invoke();
    }
    
    public void FastComplete(string analyticsLocation)
    {
        if (IsFree())
        {
            NSAudioService.Current.Play(SoundId.TimeBoost);
            Complete();
            return;
        }

        var currentPrice = GetPrice();
        
        CurrencyHelper.Purchase(Currency.Timer.Name, 1, currentPrice, success =>
        {
            if(success == false) return;
            
            NSAudioService.Current.Play(SoundId.TimeBoost);
            Analytics.SendPurchase(analyticsLocation, "item1", new List<CurrencyPair>{currentPrice}, null, false, false);
            
            Complete();
        });
    }
    
    public CurrencyPair GetPrice()
    {
        var timeLeft = Mathf.CeilToInt((float)CompleteTime.GetTimeLeft(UseUTC).TotalSeconds / 60f);
        price.Amount = Mathf.Max(1, CalcPriceByLeftTime((float)timeLeft));
        return price;
    }

    private int CalcPriceByLeftTime(float leftTime)
    {
        var coefficient = CalcCoefficient(leftTime);
        var price = Mathf.CeilToInt(Mathf.Pow(leftTime, coefficient));

        return price;
    }
    
    private float CalcCoefficient(float leftTime)
    {
        if(leftTime <= 60)
            return Mathf.Log(GameDataService.Current.ConstantsManager.HourBasePrice, 60);
        return Mathf.Log(GameDataService.Current.ConstantsManager.DayBasePrice, 24 * 60);
    }
    
    public bool IsFree()
    {
        return CompleteTime.GetTimeLeft(UseUTC).TotalSeconds <= GameDataService.Current.ConstantsManager.FreeTimeLimit;
    }
}