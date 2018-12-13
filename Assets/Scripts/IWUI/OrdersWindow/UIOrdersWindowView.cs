using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIOrdersWindowView : UIGenericPopupWindowView
{
    [IWUIBinding("#MessageIngredients")] private NSText messageIngredients;
    [IWUIBinding("#MessageOrders")] private NSText messageOders;
    
    [IWUIBinding("#TabOrders")] private GameObject tabOders;
    [IWUIBinding("#TabRecipes")] private GameObject tabRecipes;
    [IWUIBinding("#TabIngredients")] private GameObject tabIngredients;
    
    [IWUIBinding("#BackSecond")] private GameObject back;
    
    [IWUIBinding("#TabOrders")] private UIContainerViewController contentSelect;
    
    [IWUIBinding("#ContentOrders")] private UIContainerViewController contentOders;
    [IWUIBinding("#ContentRecipes")] private UIContainerViewController contentRecipes;
    [IWUIBinding("#ContentIngredients")] private UIContainerViewController contentIngredients;
    
    [IWUIBinding("#Prices")] private UIOrderPriceItem prices;
    
    
    
    
    [SerializeField] private NSText ordersLabelOn;
    [SerializeField] private NSText ordersLabelOff;
    
    [SerializeField] private NSText recipesLabelOn;
    [SerializeField] private NSText recipesLabelOff;
    
    [SerializeField] private NSText ingredientsLabelOn;
    [SerializeField] private NSText ingredientsLabelOff;
    
    [SerializeField] private Toggle ordersToggle;
    
    public override void InitView(IWWindowModel model, IWWindowController controller)
    {
        base.InitView(model, controller);
        
        var windowModel = Model as UIOrdersWindowModel;

        Fill(UpdateEntitiesSelect(null), contentSelect);
        Fill(UpdateEntitiesOders(windowModel.Orders), contentOders);
        Fill(UpdateEntitiesRecipes(windowModel.Recipes), contentRecipes);
        Fill(UpdateEntitiesIngredients(windowModel.Ingredients), contentIngredients);
    }

    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UIOrdersWindowModel;
        
        SetTitle(windowModel.Title);

        messageOders.Text = windowModel.OrdersMessage;
        messageIngredients.Text = windowModel.IngredientsMessage;
        
        ordersLabelOn.Text = ordersLabelOff.Text = windowModel.OrdersText;
        recipesLabelOn.Text = recipesLabelOff.Text = windowModel.RecipesText;
        ingredientsLabelOn.Text = ingredientsLabelOff.Text = windowModel.IngredientsText;

        ordersToggle.isOn = true;
        
        var dataOrders = windowModel.Orders.FindAll(order => order.State != OrderState.Init);

        UpdateOrders();
        
        windowModel.Select = null;
        
        messageOders.gameObject.SetActive(dataOrders.Count == 0);
        messageIngredients.gameObject.SetActive(windowModel.Ingredients.Count == 0);
        
        UpdateLists();
    }

    public void UpdateOrders()
    {
        var windowModel = Model as UIOrdersWindowModel;
        var dataOrders = windowModel.Orders.FindAll(order => order.State != OrderState.Init);
        var index = Mathf.Max(0, dataOrders.FindIndex(order => order == windowModel.Select));
        
        Fill(UpdateEntitiesOders(dataOrders), contentOders);
        contentOders.Select(index);
    }

    private void UpdateLists()
    {
        var windowModel = Model as UIOrdersWindowModel;
        
        Fill(UpdateEntitiesRecipes(windowModel.Recipes), contentRecipes);
        
        UpdateScrollRect(contentOders);
        UpdateScrollRect(contentRecipes);
        UpdateScrollRect(contentIngredients);
    }
    
    public void UpdateIngredients()
    {
        var windowModel = Model as UIOrdersWindowModel;
        
        Fill(UpdateEntitiesIngredients(windowModel.Ingredients), contentIngredients);
    }

    private void UpdateScrollRect(UIContainerViewController content)
    {
        var scrollRect = content.GetScrollRect();
        
        if (scrollRect != null) scrollRect.verticalNormalizedPosition = 1f;
    }
    
    private List<IUIContainerElementEntity> UpdateEntitiesSelect(Order entities)
    {
        var views = new List<IUIContainerElementEntity>();
        
        if (entities == null) return views;
        
        var entity = new UIOrderElementEntity
        {
            LabelText = string.Format(LocalizationService.Get("common.message.reward", "common.message.reward:{0}"), $"{Order.Separator}{entities.Reward}"),
            Data = entities,
            OnSelectEvent = null,
            OnDeselectEvent = null
        };
        
        views.Add(entity);
        
        return views;
    }
    
    private List<IUIContainerElementEntity> UpdateEntitiesOders(List<Order> entities)
    {
        var views = new List<IUIContainerElementEntity>(entities.Count);
        
        for (var i = 0; i < entities.Count; i++)
        {
            var def = entities[i];
            
            var entity = new UIOrderElementEntity
            {
                ContentId = def.Def.Uid,
                LabelText = def.Reward.Replace(Order.Separator, "\n"),
                Data = def,
                OnSelectEvent = (view) =>
                {
                    var orderEntity = view.Entity as UIOrderElementEntity;
                    
                    Fill(UpdateEntitiesSelect(orderEntity?.Data), contentSelect);
                },
                OnDeselectEvent = null
            };
            
            views.Add(entity);
        }
        
        return views;
    }
    
    private List<IUIContainerElementEntity> UpdateEntitiesRecipes(List<OrderDef> entities)
    {
        var views = new List<IUIContainerElementEntity>(entities.Count);
        
        for (var i = 0; i < entities.Count; i++)
        {
            var def = entities[i];
            
            var entity = new UIRecipeElementEntity
            {
                ContentId = def.Uid,
                LabelText = string.Format(LocalizationService.Get("common.message.level", "common.message.level {0}"), def.Level),
                IsLock = def.Level > GameDataService.Current.LevelsManager.Level,
                Prices = def.Prices,
                OnSelectEvent = (view) =>
                {
                    var recipeEntity = view.Entity as UIRecipeElementEntity;
                    prices.Init(recipeEntity.Prices, transform);
                },
                OnDeselectEvent = null
            };
            
            views.Add(entity);
        }
        
        return views;
    }
    
    private List<IUIContainerElementEntity> UpdateEntitiesIngredients(List<CurrencyPair> entities)
    {
        var views = new List<IUIContainerElementEntity>(entities.Count);
        
        for (var i = 0; i < entities.Count; i++)
        {
            var def = entities[i];
            
            var entity = new UISimpleScrollElementEntity
            {
                ContentId = def.Currency,
                LabelText = def.Amount.ToString(),
                OnSelectEvent = null,
                OnDeselectEvent = null
            };
            
            views.Add(entity);
        }
        
        return views;
    }
}
