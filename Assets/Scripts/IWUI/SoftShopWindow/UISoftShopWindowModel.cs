using System.Collections.Generic;

public class UISoftShopWindowModel : IWWindowModel
{
    public CurrencyDef ShopType;
    
    public string Title => LocalizationService.Get("window.shop.energy.title", "window.shop.energy.title");
    public string Message => LocalizationService.Get("You need Energy for removing obstacles", "You need Energy for removing obstacles 22 ");

    public List<ShopDef> Products => GameDataService.Current.ShopManager.Defs[ShopType.Name];
}
