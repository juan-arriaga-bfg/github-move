using System;
using Debug = IW.Logger;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;

public class UIOrdersWindowView : UIGenericPopupWindowView
{
    [IWUIBinding("#MessageIngredients")] private NSText messageIngredients;
    [IWUIBinding("#MessageOrders")] private NSText messageOders;
    
    [IWUIBinding("#TabOrders")] private GameObject tabOrders;
    [IWUIBinding("#TabRecipes")] private GameObject tabRecipes;
    [IWUIBinding("#TabIngredients")] private GameObject tabIngredients;
    
    [IWUIBinding("#BackSecond")] private GameObject back;
    
    [IWUIBinding("#TabOrders")] private UIContainerViewController contentSelect;
    
    [IWUIBinding("#Toggles")] private UIContainerViewController contentToggles;
    
    [IWUIBinding("#ContentOrders")] private UIContainerViewController contentOrders;
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
        Fill(UpdateEntitiesOrders(windowModel.Orders), contentOrders);
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
        
        var windowModel = Model as UIOrdersWindowModel;

        InitButtonBase(btnMaskTop, Controller.CloseCurrentWindow);
        InitButtonBase(btnMaskBottom, Controller.CloseCurrentWindow);
        
        IsShowComplete = true;
        
        TackleBoxEvents.SendOrdersOpen();

        if (windowModel.IsHighlightToken == false) return;
        
        Highlight();
        windowModel.IsHighlightToken = false;
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
            select = Mathf.Max(0, windowModel.Orders.FindIndex(order => order == windowModel.Select));
            windowModel.Select = null;
        }
        
        contentOrders.Select(select);
    }

    private void Highlight()
    {
        foreach (UIOrderElementViewController tab in contentOrders.Tabs)
        {
            tab.HighlightToken();
        }
    }
    
    private void OnSelectToggle(int index)
    {
        var windowModel = Model as UIOrdersWindowModel;
        
        tabOrders.SetActive(index == 0);
        tabRecipes.SetActive(index == 1);
        tabIngredients.SetActive(index == 2);
        
        back.SetActive(index != 0);
        
        foreach (var order in windowModel.Orders)
        {
            DOTween.Kill(order, true);
        }
        
        switch (index)
        {
            case 0:
                messageOders.gameObject.SetActive(windowModel.Orders.Count == 0);

                if (windowModel.Orders.Count == 0) Fill(UpdateEntitiesSelect(null), contentSelect);
                
                Fill(UpdateEntitiesOrders(windowModel.Orders), contentOrders);
                contentOrders.Select(0);
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
            OnDeselectEvent = null,
            OnOrderStageChangeFromTo = OnOrderStageChangeFromTo
        };
        
        views.Add(entity);
        
        return views;
    }
    
    private List<IUIContainerElementEntity> UpdateEntitiesOrders(List<Order> entities)
    {
        var views = new List<IUIContainerElementEntity>(entities.Count);
        
        for (var i = 0; i < entities.Count; i++)
        {
            var def = entities[i];
            var dataArray = def.Reward.Replace(Order.Separator, ":").Split(new[] {":"}, StringSplitOptions.RemoveEmptyEntries);
            var rewardStr = $"{dataArray[0]} {dataArray[1]}{(dataArray.Length > 2 ? $"\n{dataArray[2]}" : "")}";
            
            var entity = new UIOrderElementEntity
            {
                ContentId = def.Def.Uid,
                LabelText = rewardStr,
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

    private void OnOrderStageChangeFromTo(Order order, OrderState fromState, OrderState toState)
    {
        
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
