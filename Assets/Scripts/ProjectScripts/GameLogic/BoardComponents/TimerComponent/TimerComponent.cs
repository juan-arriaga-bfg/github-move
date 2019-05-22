using System;
using System.Collections.Generic;
using BfgAnalytics;
using DG.Tweening;
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

    private int lastProcessedSecond = int.MinValue;  
    
    public void OnRegisterEntity(ECSEntity entity)
    {
    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }
    
    public void Start()
    {
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

    public void Add(int value, float duration = 0)
    {
        if (Delay == 0 || IsStarted == false) return;

        var then = value;
        var animationTime = StartTime;
        
        void Callback()
        {
            DOTween
                .To(() => value, (v) => { value = v; }, 0, 1.5f)
                .OnStart(() =>
                {
                    if (View != null) View.Attention();
                })
                .OnUpdate(() =>
                {
                    var step = then - value;

                    then = value;
                    animationTime = animationTime.AddSeconds(step);
                    CompleteTime = animationTime.AddSeconds(Delay);
                    OnTimeChanged?.Invoke();
                })
                .OnComplete(() => { IsPaused = false; });
        }
        
        IsPaused = true;
        StartTime = StartTime.AddSeconds(value);

        if (duration > 0)
        {
            DOTween.Sequence()
                .AppendInterval(duration)
                .AppendCallback(Callback);
            
            return;
        }

        Callback();
    }

    public void Subtract(int value, float duration = 0)
    {
        Add(-Mathf.Min(Delay, value), duration);
    }

    public void Reset()
    {
        var secureTime = SecuredTimeService.Current;
        
        StartTime = UseUTC ? secureTime.UtcNow : secureTime.Now;
        CompleteTime = StartTime.AddSeconds(Delay);
        
        IsStarted = true;
        IsPaused = false;
    }
    
    public void Stop()
    {
        IsStarted = false;
        OnStop?.Invoke();
    }
    
    public void Execute()
    {
        OnExecute?.Invoke();
        
        int elapsedSeconds = (int) StartTime.GetTime(UseUTC).TotalSeconds;

        if (lastProcessedSecond == int.MinValue || lastProcessedSecond != elapsedSeconds)
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
    
    public void FastComplete(string analyticsLocation, string reason)
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
            Analytics.SendPurchase(analyticsLocation, reason, new List<CurrencyPair>{currentPrice}, null, false, false);
            
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
        return leftTime <= 60
            ? Mathf.Log(GameDataService.Current.ConstantsManager.HourBasePrice, 60)
            : Mathf.Log(GameDataService.Current.ConstantsManager.DayBasePrice, 24 * 60);
    }
    
    public bool IsFree()
    {
        return (int) StartTime.GetTime(UseUTC).TotalSeconds < Delay && CompleteTime.GetTimeLeft(UseUTC).TotalSeconds <= GameDataService.Current.ConstantsManager.FreeTimeLimit;
    }
}