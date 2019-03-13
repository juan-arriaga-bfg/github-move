public class UIOfferWindowModel : IWWindowModel
{
    public string Title => LocalizationService.Get("window.offer.title", "window.offer.title");
    public string Button => LocalizationService.Get("common.button.buyNow", "common.button.buyNow");
    
    public ShopDef Product;
    public int ProductIndex;
    
    public string PriceFake
    {
        get
        {
            var price = (IapService.Current.GetPriceAsNumber(Product.PurchaseKey) * 1000000 * (1 + Product.Sale * 0.01f)) / 1000000;

            if (price > 0) return IapService.Current.GetLocalizedPriceStr(Product.PurchaseKey).Replace(IapService.Current.GetPriceAsNumber(Product.PurchaseKey).ToString(), price.ToString());
            
            price = Product.Price.Amount * (1 + Product.Sale * 0.01f);
            return $"${price}";
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
