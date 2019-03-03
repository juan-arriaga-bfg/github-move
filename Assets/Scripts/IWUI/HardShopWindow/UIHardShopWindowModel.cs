public class UIHardShopWindowModel : UIShopWindowModel 
{
    protected override CurrencyDef ShopType => Currency.Crystals;

    public override string AnalyticLocation => $"shop_premium";
}