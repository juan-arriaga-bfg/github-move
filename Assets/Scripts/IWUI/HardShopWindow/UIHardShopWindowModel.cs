using System.Collections.Generic;

public class UIHardShopWindowModel : IWWindowModel 
{
    public string Title => LocalizationService.Get($"window.shop.{Currency.Crystals.Name.ToLower()}.title", $"window.shop.{Currency.Crystals.Name.ToLower()}.title");
    public string Message => LocalizationService.Get($"window.shop.{Currency.Crystals.Name.ToLower()}.message", $"window.shop.{Currency.Crystals.Name.ToLower()}.message");

    public List<ShopDef> Products => GameDataService.Current.ShopManager.Defs[Currency.Crystals.Name];
}
