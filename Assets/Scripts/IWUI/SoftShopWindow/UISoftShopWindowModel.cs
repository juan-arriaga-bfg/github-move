using System.Collections.Generic;

public class UISoftShopWindowModel : IWWindowModel
{
    public CurrencyDef ShopType;
    
    public string Title => LocalizationService.Get($"window.shop.{ShopType.Name.ToLower()}.title", $"window.shop.{ShopType.Name.ToLower()}.title");
    public string Message => LocalizationService.Get($"window.shop.{ShopType.Name.ToLower()}.message", $"window.shop.{ShopType.Name.ToLower()}.message");

    public List<ShopDef> Products => GameDataService.Current.ShopManager.Defs[ShopType.Name];
}
