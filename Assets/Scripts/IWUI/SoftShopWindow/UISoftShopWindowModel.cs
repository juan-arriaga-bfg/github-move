public class UISoftShopWindowModel : UIShopWindowModel
{
    protected override CurrencyDef ShopType => Currency.Coins;
    
    public override string AnalyticLocation => $"shop_soft";
}