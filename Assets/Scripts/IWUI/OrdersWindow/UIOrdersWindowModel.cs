using System.Collections.Generic;

public class UIOrdersWindowModel : IWWindowModel 
{
    public string Title => LocalizationService.Instance.Manager.GetTextByUid("window.orders.title", "Orders");
    
    public string OrdersMessage => LocalizationService.Instance.Manager.GetTextByUid("window.orders.message.orders.empty", "You have no orders at the moment. Come back later.");
    public string IngredientsMessage => LocalizationService.Instance.Manager.GetTextByUid("window.orders.message.ingredients.empty", "You have no ingredients at the moment. Come back later.");
    
    public string OrdersText => LocalizationService.Instance.Manager.GetTextByUid("window.orders.toggle.orders", "Orders");
    public string RecipesText => LocalizationService.Instance.Manager.GetTextByUid("window.orders.toggle.recipes", "Recipes");
    public string IngredientsText => LocalizationService.Instance.Manager.GetTextByUid("window.orders.toggle.ingredients", "Ingredients");
    
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