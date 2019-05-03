public class UIEventSubscriptionWindowModel : IWWindowModel 
{
    public string Title => LocalizationService.Get("window.subscription.title", "window.subscription.title");
    public string Message => LocalizationService.Get("window.subscription.message", "window.subscription.message");
    public string ButtonText => LocalizationService.Get("common.button.ok", "common.button.ok");
}