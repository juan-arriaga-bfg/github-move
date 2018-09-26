﻿using System;
using UnityEngine;

public class EnergyCurrencyLogicComponent : LimitCurrencyLogicComponent, IECSSystem
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    private TimerComponent timer;
    public TimerComponent Timer => timer ?? (timer = GetComponent<TimerComponent>(TimerComponent.ComponentGuid));

    public int Delay { get; set; }
    
    public Action OnExecute;
    
    public long LastUpdate => Timer.StartTime.ConvertToUnixTime();

    public override void OnRegisterEntity(ECSEntity entity)
    {
        targetItem = ProfileService.Current.Purchases.GetStorageItem(Currency.Energy.Name);
        limitItem = ProfileService.Current.Purchases.GetStorageItem(Currency.EnergyLimit.Name);
        
        Timer.Delay = Delay;
        Timer.OnComplete += StepComplete;
        Timer.StartTime = DateTime.Now;
        Timer.OnExecute += TimerStopCheck;
        
        base.OnRegisterEntity(entity);
    }

    private void TimerStopCheck()
    {
        if(!CheckIsNeed() && Timer.IsExecuteable()) Timer.Stop();
    }
    
    protected override void InitInSave()
    {
        var save = ProfileService.Current.GetComponent<CurrencySaveComponent>(CurrencySaveComponent.ComponentGuid);
        
        if(save == null) return;
        
        var refil = DateTimeExtension.CountOfStepsPassedWhenAppWasInBackground(save.EnergyLastUpdate, Delay, out Timer.StartTime);
        
        targetItem.Amount += Mathf.Min(refil, limitItem.Amount - targetItem.Amount);
        
        if(CheckIsNeed())
            Timer.Start(Timer.StartTime);
    }

    public bool CheckIsNeed()
    {
        return targetItem.Amount < limitItem.Amount;
    }
    
    public void Execute()
    {
        if (!Timer.IsStarted && CheckIsNeed()) Timer.Start();
        
        OnExecute?.Invoke();
    }

    public void StepComplete()
    {
        Add(1);
        
        if(targetItem.Amount >= limitItem.Amount) Timer.Stop();
        else Timer.Start();
        
        Execute();
    }

    public object GetDependency()
    {
        return null;
    }

    public bool IsExecuteable()
    {
        return CheckIsNeed();
    }
}