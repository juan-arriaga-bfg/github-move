using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIOrdersWindowView : UIGenericPopupWindowView
{
    [SerializeField] private NSText ordersLabelOn;
    [SerializeField] private NSText ordersLabelOff;
    
    [SerializeField] private NSText recipesLabelOn;
    [SerializeField] private NSText recipesLabelOff;

    [SerializeField] private Toggle ordersToggle;
    [SerializeField] private Toggle recipesToggle;
    
    [SerializeField] private ScrollRect ordersScroll;
    [SerializeField] private ScrollRect recipesScroll;
    
    [SerializeField] private GameObject selectItem;
    [SerializeField] private GameObject patternOrder;
    [SerializeField] private GameObject patternRecipe;

    private List<UIRecipeItem> recipes = new List<UIRecipeItem>();
    private List<UIOrderItem> orders = new List<UIOrderItem>();

    public override void InitView(IWWindowModel model, IWWindowController controller)
    {
        base.InitView(model, controller);
        
        UIOrdersWindowModel windowModel = Model as UIOrdersWindowModel;
        
        var data = windowModel.Recipes;
        
        foreach (var recipe in data)
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
        
        UIOrdersWindowModel windowModel = Model as UIOrdersWindowModel;
        
        SetTitle(windowModel.Title);
        SetMessage(windowModel.Message);

        ordersLabelOn.Text = ordersLabelOff.Text = windowModel.OrdersText;
        recipesLabelOn.Text = recipesLabelOff.Text = windowModel.RecipesText;

        ordersToggle.isOn = !windowModel.IsRecipes;
        recipesToggle.isOn = windowModel.IsRecipes;
        
        var data = windowModel.Orders.FindAll(order => order.State != OrderState.Init);
        
        foreach (var order in data)
        {
            var item = Instantiate(patternOrder, patternOrder.transform.parent).GetComponent<UIOrderItem>();
            
            item.Init(order);
            orders.Add(item);
        }

        windowModel.Select = null;
        patternOrder.SetActive(false);
        
        message.gameObject.SetActive(data.Count == 0);
        selectItem.SetActive(data.Count > 0);
        
        UpdateLists();
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        UIOrdersWindowModel windowModel = Model as UIOrdersWindowModel;
    }

    public override void OnViewCloseCompleted()
    {
        base.OnViewCloseCompleted();
        
        foreach (var item in orders)
        {
            Destroy(item.gameObject);
        }
        
        orders = new List<UIOrderItem>();
        
        patternOrder.SetActive(true);
        patternRecipe.SetActive(true);
        
        patternRecipe.GetComponent<Toggle>().isOn = true;
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
        UIOrdersWindowModel windowModel = Model as UIOrdersWindowModel;
        
        var data = windowModel.Recipes;

        for (var i = 0; i < data.Count; i++)
        {
            recipes[i].Init(data[i]);
        }
        
        patternRecipe.SetActive(false);
        
        ordersScroll.verticalNormalizedPosition = 1;
        recipesScroll.verticalNormalizedPosition = 1;
    }

    public void ResetRecipesView(bool isOn)
    {
        if (isOn == false)
        {
            UpdateLists();
            return;
        }
        
        patternRecipe.SetActive(isOn);
        patternRecipe.GetComponent<Toggle>().isOn = isOn;
    }
}
