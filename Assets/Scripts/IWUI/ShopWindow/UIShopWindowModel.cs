using System.Collections.Generic;

public class UIShopWindowModel : IWWindowModel 
{
    protected virtual CurrencyDef ShopType { get; set; }

    public virtual string AnalyticLocation => $"shop_{ShopType.Name.ToLower()}";
    
    public string AnalyticReason(ShopDef def)
    {
        var isOffer = def.Products.Count > 1;

        var index = isOffer
            ? (int) ProfileService.Current.GetStorageItem(Currency.Offer.Name).Amount
            : GameDataService.Current.ShopManager.Defs[ShopType.Name].IndexOf(def) + 1;
        
        return $"item{index}";
    }


    public string Title => LocalizationService.Get($"window.shop.{ShopType.Name.ToLower()}.title", $"window.shop.{ShopType.Name.ToLower()}.title");
    public string Message => LocalizationService.Get($"window.shop.{ShopType.Name.ToLower()}.message", $"window.shop.{ShopType.Name.ToLower()}.message");

    public virtual List<ShopDef> Products => GameDataService.Current.ShopManager.Defs[ShopType.Name];
}
