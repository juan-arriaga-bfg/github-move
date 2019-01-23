using System;

public class UIConfirmationWindowModel : IWWindowModel 
{
    public string Title => LocalizationService.Get("window.confirmation.title", "window.confirmation.title");
    public string Message => LocalizationService.Get("window.confirmation.message", "window.confirmation.message");
    
    public string Button => string.Format(LocalizationService.Get("common.button.buyFor", "common.button.buyFor {0}"), Price.ToStringIcon());

    public bool IsMarket;

    public string Icon;
    
    public CurrencyPair Product;
    public CurrencyPair Price;

    public Action OnAcceptTap;
    
    public Action OnAccept;
    public Action OnCancel;
}