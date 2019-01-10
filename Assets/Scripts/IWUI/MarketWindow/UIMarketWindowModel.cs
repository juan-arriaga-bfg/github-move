public class UIMarketWindowModel : IWWindowModel 
{
    public string Title => LocalizationService.Get("window.market.title", "window.market.title");
    public string Message => LocalizationService.Get("window.market.message", "window.market.message");

    public string ButtonReset
    {
        get
        {
            var price = BoardService.Current.FirstBoard.MarketLogic.Timer.GetPrise();
            
            return string.Format(LocalizationService.Get("window.market.button.reset", "window.market.button.reset {0}"), price.ToStringIcon());
        }
    }
}