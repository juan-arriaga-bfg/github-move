using System.Collections.Generic;

public class UIEnergyShopWindowModel : IWWindowModel 
{
    public string Title => LocalizationService.Get($"window.shop.energy.title", $"window.shop.energy.title");
    public string Message => LocalizationService.Get($"window.shop.energy.message", $"window.shop.energy.message");

    public List<ShopDef> Products
    {
        get
        {
            var defs = new List<ShopDef>(GameDataService.Current.ShopManager.Defs[Currency.Energy.Name]);
            var coins = ProfileService.Current.GetStorageItem(Currency.Coins.Name).Amount;
            var index = defs.FindIndex(def => def.Products[0].Amount == 1);
            var allin = defs[index].Copy();
            var amount = coins / allin.Price.Amount;

            defs.RemoveAt(index);

            if (amount == 0) return defs;

            var price = amount * allin.Price.Amount;
            
            if(price >= defs[index].Price.Amount) return defs;

            allin.Products[0].Amount = amount;
            allin.Price.Amount = price;
            
            defs.Insert(index, allin);
            
            return defs;
        }
    }
}