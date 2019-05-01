using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class UIEventWindowView : UIGenericPopupWindowView 
{
    [IWUIBinding("#Content")] private UIContainerViewController content;
    
    [IWUIBinding("#VIPLabel")] private NSText vipLabel;
    [IWUIBinding("#MainProgressLabel")] private NSText progressLabel;
    [IWUIBinding("#MainTimerLabel")] private NSText timerLabel;
    [IWUIBinding("#ButtonShowLabel")] private NSText btnShowLabel;
    
    [IWUIBinding("#ButtonShow")] private UIButtonViewController btnShow;
    [IWUIBinding("#ButtonPremium")] private UIButtonViewController btnPremium;
    
    [IWUIBinding("#CloseMaskLeft")] private UIButtonViewController btnMaskLeft;
    [IWUIBinding("#CloseMaskRight")] private UIButtonViewController btnMaskRight;
    
    [IWUIBinding("#MainProgressLine")] private RectTransform mainLine;
    [IWUIBinding("#ProgressLine")] private RectTransform secondLine;

    private readonly AmountRange mainProgressBorder = new AmountRange(10, 145);
    private readonly AmountRange secondProgressBorder = new AmountRange(35, 100);
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UIEventWindowModel;
        
        SetTitle(windowModel.Title);
        SetMessage(windowModel.Message);

        vipLabel.Text = windowModel.VIPText;
        btnShowLabel.Text = windowModel.ButtonText;
        
        if(windowModel.Timer != null) windowModel.Timer.OnTimeChanged += OnTimeChanged;
        OnTimeChanged();
        
        Fill(UpdateEntities(), content);

        var manager = GameDataService.Current.EventManager;
        var defs = manager.Defs[EventName.OrderSoftLaunch];
        var step = manager.Step;
        
        var target = defs[defs.Count - 1].RealPrices[0].Amount;
        var real = ProfileService.Current.GetStorageItem(Currency.Token.Name).Amount;
        var current = (step == 0 ? 0 : defs[step - 1].RealPrices[0].Amount) + (manager.IsCompleted(EventName.OrderSoftLaunch) ? 0 : (int) real);
        var progress = Mathf.Clamp(mainProgressBorder.Max * (current / (float) target) + mainProgressBorder.Min, mainProgressBorder.Min, mainProgressBorder.Max);

        var progressSecond = (secondProgressBorder.Max + secondProgressBorder.Min) * step + (manager.IsCompleted(EventName.OrderSoftLaunch) ? 0 : (int) (secondProgressBorder.Max * (real / (float) manager.Price(EventName.OrderSoftLaunch))));
        
        progressLabel.Text = $"{current}/{target}";
        mainLine.sizeDelta = new Vector2(progress, mainLine.sizeDelta.y);
        secondLine.sizeDelta = new Vector2(progressSecond, secondLine.sizeDelta.y);
        
        content.GetScrollRect().horizontalNormalizedPosition = step == 0 ? 1 : 0;
    }

    public override void OnViewShowCompleted()
    {
        base.OnViewShowCompleted();
        
        InitButtonBase(btnShow, OnShowClick);
        InitButtonBase(btnPremium, OnPremiumClick);
        
        InitButtonBase(btnMaskLeft, Controller.CloseCurrentWindow);
        InitButtonBase(btnMaskRight, Controller.CloseCurrentWindow);

        if (GameDataService.Current.EventManager.Step == 0)
        {
            Scroll(0);
            return;
        }
        
        for (var i = 0; i < content.Tabs.size; i++)
        {
            var tab = content.Tabs[i] as UIEventElementViewController;
            
            if (tab.IsComplete) continue;
            if (i <= 1) return;
            
            Scroll(tab.Index);
            return;
        }
        
        Scroll(content.Tabs.size - 1);
    }
    
    public override void OnViewCloseCompleted()
    {
        var windowModel = Model as UIEventWindowModel;
        
        if(windowModel.Timer != null) windowModel.Timer.OnTimeChanged -= OnTimeChanged;
        
        base.OnViewCloseCompleted();
    }
    
    private List<IUIContainerElementEntity> UpdateEntities()
    {
        var defs = GameDataService.Current.EventManager.Defs[EventName.OrderSoftLaunch];
        var views = new List<IUIContainerElementEntity>(defs.Count);
        
        foreach (var def in defs)
        {
            var entity = new UIEventElementEntity
            {
                Step = def,
                OnSelectEvent = null,
                OnDeselectEvent = null
            };
            
            views.Add(entity);
        }
        
        return views;
    }
    
    private void Scroll(int index)
    {
        DOTween.Kill(content);
        
        var posX = Mathf.Clamp((-content.CachedRectTransform.sizeDelta.x / content.Tabs.size) * index, -content.CachedRectTransform.sizeDelta.x, 0);

        if (Mathf.Abs(content.CachedRectTransform.anchoredPosition.x - posX) <= 0.01f) return;
        
        const float duration = 1.5f;
        var percent = Mathf.Abs(content.CachedRectTransform.anchoredPosition.x / content.CachedRectTransform.sizeDelta.x);
        
        content.GetScrollRect().enabled = false;
        content.CachedRectTransform.DOAnchorPosX(posX, duration*percent + duration*(1-percent))
            .SetEase(Ease.InOutBack)
            .SetId(content)
            .OnComplete(() => { content.GetScrollRect().enabled = true; });
    }

    private void OnShowClick()
    {
        var model = UIService.Get.GetCachedModel<UIOrdersWindowModel>(UIWindowType.OrdersWindow);

        if (model.Orders != null && model.Orders.Count > 0) model.Select = model.Orders[0];

        model.IsHighlightToken = true;
        UIService.Get.ShowWindow(UIWindowType.OrdersWindow);
    }

    private void OnPremiumClick()
    {
        UIService.Get.ShowWindow(UIWindowType.EventSubscriptionWindow);
    }
    
    private void OnTimeChanged()
    {
        var windowModel = Model as UIEventPreviewWindowModel;

//        timerLabel.Text = windowModel.TimerText;
    }
}
