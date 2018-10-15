using System.Collections.Generic;

public class UIOrdersWindowModel : IWWindowModel 
{
    public string Title => "Orders";
    
    public string Message => "You have no orders at the moment. Come back later.";
    
    public string OrdersText => "Orders";
    public string RecipesText => "Recipes";

    public bool IsRecipes;

    public List<OrderDef> Recipes => GameDataService.Current.OrdersManager.Recipes;
    public List<Order> Orders => GameDataService.Current.OrdersManager.Orders;
}