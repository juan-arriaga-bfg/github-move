using System.Collections.Generic;

public class ShopDef
{
    public string Uid;
    public string Icon;
    public string Name;
    public string PurchaseKey;
    public List<CurrencyPair> Products;
    public CurrencyPair Price;
    public bool IsPermanent;
    
    
    public ShopDef Copy()
    {
        var products = new List<CurrencyPair>();

        foreach (var product in Products)
        {
            products.Add(new CurrencyPair{Currency = product.Currency, Amount = product.Amount});
        }
        
        return new ShopDef
        {
            Uid = this.Uid,
            Icon = this.Icon,
            Name = this.Name,
            PurchaseKey = this.PurchaseKey,
            Price = new CurrencyPair{Currency = this.Price.Currency, Amount = this.Price.Amount},
            Products = products,
            IsPermanent = this.IsPermanent
        };
    }
}