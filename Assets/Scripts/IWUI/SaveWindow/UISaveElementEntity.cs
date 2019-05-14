using System;
using System.Collections.Generic;
using UnityEngine;

public class UISaveElementEntity : UISimpleScrollElementEntity
{
    public bool IsLocal;
    
    public string Currencies;

    private string dateText;
    public override string LabelText => dateText;
    
    public string Level;
    
    public string Button => LocalizationService.Get($"window.progress.save.button.{(IsLocal ? "device" : "server")}", $"window.progress.save.button.{(IsLocal ? "device" : "server")}");

    public Action OnAccept;
    
    public void Parce(bool isLocal, int level, List<CurrencyPair> currencies, DateTime date)
    {
        IsLocal = isLocal;
        Level = string.Format(LocalizationService.Get("common.message.level", "common.message.level {0}"), level);
        
        dateText = date == DateTime.Now
            ? LocalizationService.Get("window.progress.save.state.current", "window.progress.save.state.current")
            : string.Format(LocalizationService.Get("window.progress.save.state.then", "window.progress.save.state.then {0}"), DateTimeExtension.TimeFormat(date.TimeOfDay,false, null, false, 3f));

        Currencies = CurrencyHelper.RewardsToString("\n", null, currencies);

        OnAccept = () => { Debug.LogError($"!!!!!!!!! {isLocal}"); };
    }
}