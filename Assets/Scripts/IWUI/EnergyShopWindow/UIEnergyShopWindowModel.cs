using System.Collections.Generic;

public class UIEnergyShopWindowModel : IWWindowModel 
{
    public string Title
    {
        get { return "Out of Energy"; }
    }
    
    public string Message
    {
        get { return "Need more energy? Make it yourself or buy more."; }
    }
    
    public string SecondMessage
    {
        get { return "You can get energy for free from:"; }
    }

    public string ButtonText
    {
        get { return "Show"; }
    }

    public List<ShopDef> Products
    {
        get { return GameDataService.Current.ShopManager.Products; }
    }
}
