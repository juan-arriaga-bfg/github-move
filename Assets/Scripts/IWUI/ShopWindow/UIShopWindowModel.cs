using System.Collections.Generic;

public class UIShopWindowModel : IWWindowModel 
{
    protected virtual CurrencyDef ShopType { get; set; }

    public virtual string AnalyticLocation => $"shop_{ShopType.Name.ToLower()}";
    
    public virtual string AnalyticReason(ShopDef def)
    {
        return $"{ShopType.Name.ToLower()}";
    }
    
    public string Title => LocalizationService.Get($"window.shop.{ShopType.Name.ToLower()}.title", $"window.shop.{ShopType.Name.ToLower()}.title");
    public string Message => LocalizationService.Get($"window.shop.{ShopType.Name.ToLower()}.message", $"window.shop.{ShopType.Name.ToLower()}.message");

    public virtual List<ShopDef> Products => GameDataService.Current.ShopManager.Defs[ShopType.Name];
}
