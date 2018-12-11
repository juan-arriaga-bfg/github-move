public class UISoftShopElementEntity : UISimpleScrollElementEntity
{
    public override string LabelText => $"+{Product.ToStringIcon()}";
    public string ButtonLabel => string.Format(LocalizationService.Get("common.button.buy", "common.button.buy {0}"), Price.ToStringIcon());
    
    public CurrencyPair Product;
    public CurrencyPair Price;
}