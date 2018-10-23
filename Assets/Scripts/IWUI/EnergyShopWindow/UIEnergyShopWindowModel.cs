using System.Collections.Generic;

public class UIEnergyShopWindowModel : IWWindowModel
{
    private List<ShopDef> products;
    
    public string Title => "Out of Energy";

    public string Message => "Need more energy? Make it yourself or buy more.";

    public string SecondMessage => "Get energy for free from:";

    public string ButtonText => "Show";

    public List<ShopDef> Products => products ?? (products = GameDataService.Current.ShopManager.Defs["Energy"]);
}
