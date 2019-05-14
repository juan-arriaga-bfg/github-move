using System;
using System.Collections.Generic;

public class UISaveWindowModel : IWWindowModel 
{
    public string Title => LocalizationService.Get("window.progress.save.title", "window.progress.save.title");
    public string Message => LocalizationService.Get("window.progress.save.message", "window.progress.save.message");

    public List<UISaveElementEntity> Profiles
    {
        get
        {
            var local = new UISaveElementEntity();
            var server = new UISaveElementEntity();
            var profiles = new List<UISaveElementEntity>{local, server};
            
            local.Parce(true, 5, new List<CurrencyPair>
            {
                new CurrencyPair{Currency = Currency.Experience.Name, Amount = 1000},
                new CurrencyPair{Currency = Currency.Coins.Name, Amount = 100},
                new CurrencyPair{Currency = Currency.Crystals.Name, Amount = 10},
            }, DateTime.Now);
            
            server.Parce(false, 15, new List<CurrencyPair>
            {
                new CurrencyPair{Currency = Currency.Experience.Name, Amount = 2000},
                new CurrencyPair{Currency = Currency.Coins.Name, Amount = 200},
                new CurrencyPair{Currency = Currency.Crystals.Name, Amount = 20},
            }, DateTime.Now.AddDays(-1));
            
            return profiles;
        }
    }
}
