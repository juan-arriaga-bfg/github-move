public class UIOfferWindowModel : IWWindowModel
{
    public string Title => LocalizationService.Get("window.offer.title", "window.offer.title");
    public string Button => LocalizationService.Get("common.button.buyNow", "common.button.buyNow");
    
    public ShopDef Product;
    
    public string PriceFake
    {
        get
        {
            var ret = IapService.Current.GetLocalizedPriceStr(Product.PurchaseKey);
            
            if (string.IsNullOrEmpty(ret)) ret = $"${Product.Price.Amount}";

            return ret;
        }
    }

    public string PriceReal
    {
        get
        {
            var ret = IapService.Current.GetLocalizedPriceStr(Product.PurchaseKey);
            
            if (string.IsNullOrEmpty(ret)) ret = $"${Product.Price.Amount}";

            return ret;
        }
    }
}
