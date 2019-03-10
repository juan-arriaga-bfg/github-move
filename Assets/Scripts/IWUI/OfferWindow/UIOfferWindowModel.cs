public class UIOfferWindowModel : IWWindowModel
{
    public string Title => LocalizationService.Get("window.offer.title", "window.offer.title");
    public string Button => LocalizationService.Get("common.button.buyNow", "common.button.buyNow");
    
    public ShopDef Product => GameDataService.Current.ShopManager.Defs["Offer"][0];
}
