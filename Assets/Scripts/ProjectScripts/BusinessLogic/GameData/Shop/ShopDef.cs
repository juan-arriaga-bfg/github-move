using System.Collections.Generic;
using System.Linq;

public class ShopDef
{
    public string Uid;
    public string Cohort;
    public string Icon;
    public string Name;
    public string PurchaseKey;
    public string SaleKey;
    public List<CurrencyPair> Products;
    public List<CurrencyPair> Extras;
    public CurrencyPair Price;
    public bool IsPermanent;
    public int Sale;
    public int[] Delays;
    
    public ShopDef Copy()
    {
        var products = new List<CurrencyPair>();
        var extras = new List<CurrencyPair>();

        foreach (var product in Products)
        {
            products.Add(new CurrencyPair{Currency = product.Currency, Amount = product.Amount});
        }

        if (Extras != null)
        {
            foreach (var extra in Extras)
            {
                extras.Add(new CurrencyPair{Currency = extra.Currency, Amount = extra.Amount});
            }
        }
        
        return new ShopDef
        {
            Uid = this.Uid,
            Icon = this.Icon,
            Name = this.Name,
            PurchaseKey = this.PurchaseKey,
            SaleKey = this.SaleKey,
            Price = new CurrencyPair{Currency = this.Price.Currency, Amount = this.Price.Amount},
            Products = products,
            Extras = extras,
            IsPermanent = this.IsPermanent,
            Sale = this.Sale,
            Delays = this.Delays?.ToArray()
        };
    }
}