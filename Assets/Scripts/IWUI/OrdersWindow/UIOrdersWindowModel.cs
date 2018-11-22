using System.Collections.Generic;

public class UIOrdersWindowModel : IWWindowModel 
{
    public string Title => LocalizationService.Get("window.orders.title", "window.orders.title");
    
    public string OrdersMessage => LocalizationService.Get("window.orders.message.orders.empty", "window.orders.message.orders.empty");
    public string IngredientsMessage => LocalizationService.Get("window.orders.message.ingredients.empty", "window.orders.message.ingredients.empty");
    
    public string OrdersText => LocalizationService.Get("window.orders.toggle.orders", "window.orders.toggle.orders");
    public string RecipesText => LocalizationService.Get("window.orders.toggle.recipes", "window.orders.toggle.recipes");
    public string IngredientsText => LocalizationService.Get("window.orders.toggle.ingredients", "window.orders.toggle.ingredients");
    
    public Order Select; 

    public List<OrderDef> Recipes => GameDataService.Current.OrdersManager.Recipes;
    public List<Order> Orders => GameDataService.Current.OrdersManager.Orders;

    public List<CurrencyPair> Ingredients
    {
        get
        {
            var currencys = new List<CurrencyPair>();
            
            for (var i = Currency.PR_A5.Id; i < Currency.PR_E5.Id + 1; i++)
            {
                var currency = Currency.GetCurrencyDef(i);
                var amound = ProfileService.Current.GetStorageItem(currency.Name).Amount;
                
                if(amound == 0) continue;
                
                currencys.Add(new CurrencyPair{Currency = currency.Name, Amount = amound});
            }
            
            return currencys;
        }
    }
}