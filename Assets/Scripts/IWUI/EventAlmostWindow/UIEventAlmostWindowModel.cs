using System.Collections.Generic;

public class UIEventAlmostWindowModel : IWWindowModel 
{
    public string Title => LocalizationService.Get("window.event.almost.title", "window.event.almost.title");
    public string Message => LocalizationService.Get("window.event.almost.message", "window.event.almost.message");
    public string Message2 => LocalizationService.Get("window.event.almost.message2", "window.event.almost.message2");
    
    public string MarkText => LocalizationService.Get("window.event.almost.mark", "window.event.almost.mark");
    public string ButtonText => string.Format(LocalizationService.Get("common.button.buy", "common.button.buy {0}"), Price.ToStringIcon());

    public int Step;
    
    public CurrencyPair Price;
    public List<CurrencyPair> Rewards;
}
