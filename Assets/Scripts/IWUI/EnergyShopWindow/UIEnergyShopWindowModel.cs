using System.Collections.Generic;

public class UIEnergyShopWindowModel : IWWindowModel
{
    private List<ShopDef> products;
    
    public string Title => LocalizationService.Instance.Manager.GetTextByUid("window.shop.energy.title", "Out of Energy");

    public List<ShopDef> Products => products ?? (products = GameDataService.Current.ShopManager.Defs["Energy"]);
}
