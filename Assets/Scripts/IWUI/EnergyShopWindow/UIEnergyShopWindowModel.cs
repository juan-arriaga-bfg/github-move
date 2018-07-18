using System.Collections.Generic;

public class UIEnergyShopWindowModel : IWWindowModel 
{
    public string Title
    {
        get { return "Shop of Energy"; }
    }

    public List<ShopDef> Products
    {
        get { return GameDataService.Current.ShopManager.Products; }
    }
}
