using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIOrdersWindowView : UIGenericPopupWindowView
{
    [SerializeField] private NSText ingredientsMessage;
    
    [SerializeField] private NSText ordersLabelOn;
    [SerializeField] private NSText ordersLabelOff;
    
    [SerializeField] private NSText recipesLabelOn;
    [SerializeField] private NSText recipesLabelOff;
    
    [SerializeField] private NSText ingredientsLabelOn;
    [SerializeField] private NSText ingredientsLabelOff;
    
    [SerializeField] private Toggle ordersToggle;
    
    [SerializeField] private ScrollRect ordersScroll;
    [SerializeField] private ScrollRect recipesScroll;
    [SerializeField] private ScrollRect ingredientsScroll;
    
    [SerializeField] private GameObject ingredientsBack;
    
    [SerializeField] private GameObject selectItem;
    [SerializeField] private GameObject patternOrder;
    [SerializeField] private GameObject patternRecipe;
    [SerializeField] private GameObject patternIngredient;

    private List<UIRecipeItem> recipes = new List<UIRecipeItem>();
    private List<UIOrderItem> orders = new List<UIOrderItem>();
    private List<UISimpleScrollItem> ingredients = new List<UISimpleScrollItem>();

    public override void InitView(IWWindowModel model, IWWindowController controller)
    {
        base.InitView(model, controller);
        
        var windowModel = Model as UIOrdersWindowModel;
        
        var dataRecipes = windowModel.Recipes;
        
        foreach (var recipe in dataRecipes)
        {
            var item = Instantiate(patternRecipe, patternRecipe.transform.parent).GetComponent<UIRecipeItem>();
            
            item.Init(recipe);
            recipes.Add(item);
        }
        
        patternRecipe.SetActive(false);
    }

    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UIOrdersWindowModel;
        
        SetTitle(windowModel.Title);
        SetMessage(windowModel.OrdersMessage);

        ingredientsMessage.Text = windowModel.IngredientsMessage;
        ordersLabelOn.Text = ordersLabelOff.Text = windowModel.OrdersText;
        recipesLabelOn.Text = recipesLabelOff.Text = windowModel.RecipesText;
        ingredientsLabelOn.Text = ingredientsLabelOff.Text = windowModel.IngredientsText;

        ordersToggle.isOn = true;
        
        var dataOrders = windowModel.Orders.FindAll(order => order.State != OrderState.Init);
        
        foreach (var order in dataOrders)
        {
            var item = Instantiate(patternOrder, patternOrder.transform.parent).GetComponent<UIOrderItem>();
            
            item.Init(order);
            orders.Add(item);
        }
        
        var dataIngredients = windowModel.Ingredients;
        
        foreach (var ingredient in dataIngredients)
        {
            var item = Instantiate(patternIngredient, patternIngredient.transform.parent).GetComponent<UISimpleScrollItem>();
            
            item.Init(ingredient.Currency, ingredient.Amount.ToString());
            ingredients.Add(item);
        }

        windowModel.Select = null;
        
        patternOrder.SetActive(false);
        patternIngredient.SetActive(false);
        
        message.gameObject.SetActive(dataOrders.Count == 0);
        ingredientsMessage.gameObject.SetActive(dataIngredients.Count == 0);
        ingredientsBack.SetActive(dataIngredients.Count != 0);
        selectItem.SetActive(dataOrders.Count > 0);
        
        UpdateLists();
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        var windowModel = Model as UIOrdersWindowModel;
    }

    public override void OnViewCloseCompleted()
    {
        base.OnViewCloseCompleted();
        
        foreach (var item in orders)
        {
            Destroy(item.gameObject);
        }
        
        foreach (var item in ingredients)
        {
            Destroy(item.gameObject);
        }
        
        orders = new List<UIOrderItem>();
        ingredients = new List<UISimpleScrollItem>();
        
        patternOrder.SetActive(true);
        patternIngredient.SetActive(true);
    }

    public void UpdateOrders()
    {
        foreach (var order in orders)
        {
            order.UpdateIndicator();
        }
    }

    private void UpdateLists()
    {
        var windowModel = Model as UIOrdersWindowModel;
        
        var data = windowModel.Recipes;

        for (var i = 0; i < data.Count; i++)
        {
            recipes[i].Init(data[i]);
        }
        
        ordersScroll.verticalNormalizedPosition = 1;
        recipesScroll.verticalNormalizedPosition = 1;
        ingredientsScroll.verticalNormalizedPosition = 1;
    }
    
    public void UpdateIngredients()
    {
        var windowModel = Model as UIOrdersWindowModel;
        
        foreach (var item in ingredients)
        {
            Destroy(item.gameObject);
        }
        
        ingredients = new List<UISimpleScrollItem>();
        patternIngredient.SetActive(true);
        
        var data = windowModel.Ingredients;
        
        foreach (var ingredient in data)
        {
            var item = Instantiate(patternIngredient, patternIngredient.transform.parent).GetComponent<UISimpleScrollItem>();
            
            item.Init(ingredient.Currency, ingredient.Amount.ToString());
            ingredients.Add(item);
        }
        
        patternIngredient.SetActive(false);
    }
}
