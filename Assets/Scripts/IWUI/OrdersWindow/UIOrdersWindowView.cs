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
    
    [IWUIBinding("#Toggles")] private UIContainerViewController contentToggles;
    
    [IWUIBinding("#ContentOrders")] private UIContainerViewController contentOders;
    [IWUIBinding("#ContentRecipes")] private UIContainerViewController contentRecipes;
    [IWUIBinding("#ContentIngredients")] private UIContainerViewController contentIngredients;
    
    [IWUIBinding("#Prices")] private UIOrderPriceItem prices;
    
    [IWUIBinding("#CloseMaskTop")] private UIButtonViewController btnMaskTop;
    [IWUIBinding("#CloseMaskBottom")] private UIButtonViewController btnMaskBottom;

    public bool IsShowComplete;
    
    public override void InitView(IWWindowModel model, IWWindowController controller)
    {
        base.InitView(model, controller);
        
        var windowModel = Model as UIOrdersWindowModel;
        
        var toggles = new List<string>
        {
            windowModel.OrdersText,
            windowModel.RecipesText,
            windowModel.IngredientsText
        };
        
        Fill(UpdateEntitiesToggles(toggles), contentToggles);
        Fill(UpdateEntitiesSelect(null), contentSelect);
        Fill(UpdateEntitiesOders(windowModel.Orders), contentOders);
        Fill(UpdateEntitiesRecipes(windowModel.Recipes), contentRecipes);
        Fill(UpdateEntitiesIngredients(windowModel.Ingredients), contentIngredients);
    }

    public override void OnViewShow()
    {
        IsShowComplete = false;
        
        base.OnViewShow();
        
        var windowModel = Model as UIOrdersWindowModel;
        
        SetTitle(windowModel.Title);

        messageOders.Text = windowModel.OrdersMessage;
        messageIngredients.Text = windowModel.IngredientsMessage;
        
        contentToggles.Select(0);
        SelectOrders();
    }

    public override void OnViewShowCompleted()
    {
        base.OnViewShowCompleted();

        InitButtonBase(btnMaskTop, Controller.CloseCurrentWindow);
        InitButtonBase(btnMaskBottom, Controller.CloseCurrentWindow);
        
        IsShowComplete = true;
        
        TackleBoxEvents.SendOrdersOpen();
    }

    public override void OnViewCloseCompleted()
    {
        base.OnViewCloseCompleted();
        TackleBoxEvents.SendOrdersClosed();
    }

    private void SelectOrders()
    {
        var windowModel = Model as UIOrdersWindowModel;
        var select = 0;
        
        if (windowModel.Select != null)
        {
            var dataOrders = windowModel.Orders.FindAll(order => order.State != OrderState.Init);
            select = Mathf.Max(0, dataOrders.FindIndex(order => order == windowModel.Select));
            windowModel.Select = null;
        }
        
        contentOders.Select(select);
    }
    
    private void OnSelectToggle(int index)
    {
        var windowModel = Model as UIOrdersWindowModel;
        
        tabOders.SetActive(index == 0);
        tabRecipes.SetActive(index == 1);
        tabIngredients.SetActive(index == 2);
        
        back.SetActive(index != 0);
        
        switch (index)
        {
            case 0:
                var dataOrders = windowModel.Orders.FindAll(order => order.State != OrderState.Init);
                
                messageOders.gameObject.SetActive(dataOrders.Count == 0);

                if (dataOrders.Count == 0) Fill(UpdateEntitiesSelect(null), contentSelect);
                
                Fill(UpdateEntitiesOders(dataOrders), contentOders);
                contentOders.Select(0);
                prices.HideHard();
                break;
            case 1:
                CachedHintArrowComponent.ClearArrows();
                Fill(UpdateEntitiesRecipes(windowModel.Recipes), contentRecipes);
                UpdateScrollRect(contentRecipes);
                break;
            case 2:
                messageIngredients.gameObject.SetActive(windowModel.Ingredients.Count == 0);
                CachedHintArrowComponent.ClearArrows();
                Fill(UpdateEntitiesIngredients(windowModel.Ingredients), contentIngredients);
                UpdateScrollRect(contentIngredients);
                prices.HideHard();
                break;
        }
    }

    private void UpdateScrollRect(UIContainerViewController content)
    {
        var scrollRect = content.GetScrollRect();
        
        if (scrollRect != null) scrollRect.verticalNormalizedPosition = 1f;
    }
    
    private List<IUIContainerElementEntity> UpdateEntitiesToggles(List<string> entities)
    {
        var views = new List<IUIContainerElementEntity>(entities.Count);
        
        for (var i = 0; i < entities.Count; i++)
        {
            var def = entities[i];
            
            var entity = new UISimpleTabContainerElementEntity
            {
                LabelText = def,
                CheckmarkText = def,
                OnSelectEvent = view =>
                {
                    OnSelectToggle(view.Index);
                },
                OnDeselectEvent = null
            };
            
            views.Add(entity);
        }
        
        return views;
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
                    prices.Select(recipeEntity.Prices, view.CachedTransform);
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
