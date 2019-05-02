using System;
using System.Collections.Generic;

public class UIExchangeWindowModel : IWWindowModel
{
    public string Title => LocalizationService.Get("window.exchange.ingredients.title", "window.exchange.ingredients.title");

    public string Message => GameDataService.Current.TutorialDataManager.CheckFirstOrder()
        ? LocalizationService.Get("window.exchange.ingredients.message", "window.exchange.ingredients.message")
        : LocalizationService.Get("window.exchange.ingredients.tutor", "window.exchange.ingredients.tutor");

    public string Button => GameDataService.Current.TutorialDataManager.CheckFirstOrder()
        ? string.Format(LocalizationService.Get("common.button.buyAll", "common.button.buyAll {0}"), Price.ToStringIcon())
        : LocalizationService.Get("common.button.find", "common.button.find");

    public Action OnClick;
    
    public CurrencyPair Price;

    public List<CurrencyPair> Products;
}
