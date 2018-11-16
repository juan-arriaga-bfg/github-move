using System.Collections.Generic;

public class UIOrdersWindowModel : IWWindowModel 
{
    public string Title => "Orders";
    
    public string Message => "You have no orders at the moment. Come back later.";
    public string TngredientsMessage => "You have no ingredients at the moment. Come back later.";
    
    public string OrdersText => "Orders";
    public string RecipesText => "Recipes";
    public string IngredientsText => "Ingredients";
    
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