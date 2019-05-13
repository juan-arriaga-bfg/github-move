using System.Collections.Generic;

public class UIEnergyShopWindowModel : UIShopWindowModel
{
    protected override CurrencyDef ShopType => Currency.Energy;

    public override string AnalyticReason(ShopDef def)
    {
        var reason = "";

        switch (GameDataService.Current.ShopManager.Defs[ShopType.Name].IndexOf(def))
        {
            case 0:
                reason = "bonus";
                break;
            case 2:
                reason = "full";
                break;
            default:
                reason = "adaptive";
                break;
        }
        
        return $"{ShopType.Name.ToLower()}_{reason}";
    }

    public override List<ShopDef> Products
    {
        get
        {
            var defs = new List<ShopDef>(base.Products);
            var coins = ProfileService.Current.GetStorageItem(Currency.Coins.Name).Amount;
            var index = defs.FindIndex(def => def.Products[0].Amount == 1);
            var allin = defs[index].Copy();
            var amount = coins / allin.Price.Amount;
            
            defs.RemoveAt(index);
            
            if (amount == 0 || coins >= defs[index].Price.Amount) return defs;

            allin.Products[0].Amount = amount;
            allin.Price.Amount = amount * allin.Price.Amount;
            
            defs.Insert(index, allin);
            
            return defs;
        }
    }
}