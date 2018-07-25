using System.Collections.Generic;

public class UIEnergyShopWindowModel : IWWindowModel
{
    private List<int> selectedPieces;
    
    
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
        get { return "Get energy for free from:"; }
    }

    public string ButtonText
    {
        get { return "Show"; }
    }

    public List<ShopDef> Products
    {
        get { return GameDataService.Current.ShopManager.Products; }
    }

    public List<int> SelectedPieces
    {
        get { return selectedPieces ?? (selectedPieces = PieceType.GetIdsByFilter(PieceTypeFilter.Energy)); }
    }
}
