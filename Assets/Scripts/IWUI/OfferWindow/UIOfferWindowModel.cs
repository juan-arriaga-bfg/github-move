public class UIOfferWindowModel : IWWindowModel
{
    public string Title => LocalizationService.Get("window.offer.title", "window.offer.title");
    public string Button => LocalizationService.Get("common.button.buyNow", "common.button.buyNow");
    
    public string PriceFake
    {
        get
        {
            var product = BoardService.Current.FirstBoard.MarketLogic.Offer;
            var ret = IapService.Current.GetLocalizedPriceStr(product.SaleKey);
            
            if (string.IsNullOrEmpty(ret)) ret = $"${product.Price.Amount * (1 + product.Sale * 0.01f)}";

            return ret;
        }
    }

    public string PriceReal
    {
        get
        {
            var product = BoardService.Current.FirstBoard.MarketLogic.Offer;
            var ret = IapService.Current.GetLocalizedPriceStr(product.PurchaseKey);
            
            if (string.IsNullOrEmpty(ret)) ret = $"${product.Price.Amount}";

            return ret;
        }
    }
}
