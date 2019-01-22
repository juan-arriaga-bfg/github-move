public class UIMarketWindowModel : IWWindowModel 
{
    public string Title => LocalizationService.Get("window.market.title", "window.market.title");
    public string Message => LocalizationService.Get("window.market.message", "window.market.message");
    
    public CurrencyPair Price = new CurrencyPair{Currency = Currency.Crystals.Name, Amount = GameDataService.Current.ConstantsManager.MarketUpdatePrice};
    
    public string ButtonReset => string.Format(LocalizationService.Get("window.market.button.reset", "window.market.button.reset {0}"), Price.ToStringIcon());

    public bool IsTutorial;
}