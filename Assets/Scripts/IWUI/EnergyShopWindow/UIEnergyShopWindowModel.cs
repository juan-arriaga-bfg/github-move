using System.Collections.Generic;

public class UIEnergyShopWindowModel : IWWindowModel
{
    private List<ShopDef> products;
    
    public string Title => LocalizationService.Get("window.shop.energy.title", "window.shop.energy.title");

    public List<ShopDef> Products => products ?? (products = GameDataService.Current.ShopManager.Defs["Energy"]);
}
