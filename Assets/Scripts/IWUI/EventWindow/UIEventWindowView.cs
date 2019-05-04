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
    
    [IWUIBinding("#MessageIcon")] private GameObject messageIcon;
    [IWUIBinding("#Message")] private RectTransform messageTransform;

    private readonly AmountRange mainProgressBorder = new AmountRange(10, 145);
    private readonly AmountRange secondProgressBorder = new AmountRange(35, 100);

    private EventGame eventGame;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        BoardService.Current.FirstBoard.BoardLogic.EventGamesLogic.GetEventGame(EventGameType.OrderSoftLaunch, out eventGame);
        
        var windowModel = Model as UIEventWindowModel;
        var isCompleted = eventGame.State == EventGameState.Complete;
        
        SetTitle(windowModel.Title);
        SetMessage(isCompleted ? windowModel.MessageFinish : windowModel.Message);

        vipLabel.Text = windowModel.VIPText;
        btnShowLabel.Text = isCompleted ? windowModel.ButtonFinishText : windowModel.ButtonText;
        
        messageIcon.SetActive(isCompleted == false);
        messageTransform.sizeDelta = new Vector2(isCompleted ? 860 : 660, messageTransform.sizeDelta.y);
        messageTransform.anchoredPosition = new Vector2(isCompleted ? -100 : 0, messageTransform.anchoredPosition.y);

        eventGame.TimeController.OnTimeChanged += OnTimeChanged;
        
        OnTimeChanged();
        
        Fill(UpdateEntities(), content);
        
        var defs = eventGame.Steps;
        var step = eventGame.Step;
        
        var target = defs[defs.Count - 1].RealPrices[0].Amount;
        var real = ProfileService.Current.GetStorageItem(Currency.Token.Name).Amount;
        var current = step == 0 ? (int) real : defs[step - 1].RealPrices[0].Amount + real - (isCompleted ? defs[step - 1].Prices[0].Amount : 0);
        var progress = Mathf.Clamp(mainProgressBorder.Max * (current / (float) target) + mainProgressBorder.Min, mainProgressBorder.Min, mainProgressBorder.Max);
        
        var progressSecondMax = (secondProgressBorder.Max + secondProgressBorder.Min) * eventGame.Steps.Count;
        var progressSecond = Mathf.Clamp((secondProgressBorder.Max + secondProgressBorder.Min) * step + (int) (secondProgressBorder.Max * (real / (float) eventGame.Price)), 0, progressSecondMax);
        
        progressLabel.Text = $"{current}/{target}";
        mainLine.sizeDelta = new Vector2(progress, mainLine.sizeDelta.y);
        secondLine.sizeDelta = new Vector2(progressSecond, secondLine.sizeDelta.y);
        
        content.GetScrollRect().horizontalNormalizedPosition = step == 0 ? 1 : 0;
    }
    
    public override void OnViewShowCompleted()
    {
        base.OnViewShowCompleted();
        
        InitButtonBase(btnShow, OnClick);
        InitButtonBase(btnPremium, OnPremiumClick);
        
        InitButtonBase(btnMaskLeft, Controller.CloseCurrentWindow);
        InitButtonBase(btnMaskRight, Controller.CloseCurrentWindow);

        if (eventGame.Step == 0)
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

    public override void OnViewClose()
    {
        if (eventGame.State == EventGameState.Complete) eventGame.Finish();
        
        base.OnViewClose();
    }
    
    public override void OnViewCloseCompleted()
    {
        eventGame.TimeController.OnTimeChanged -= OnTimeChanged;
        
        base.OnViewCloseCompleted();
    }
    
    private List<IUIContainerElementEntity> UpdateEntities()
    {
        var views = new List<IUIContainerElementEntity>(eventGame.Steps.Count);
        
        foreach (var def in eventGame.Steps)
        {
            var entity = new UIEventElementEntity
            {
                GameStep = def,
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

    private void OnClick()
    {
        Controller.CloseCurrentWindow();
        
        if (eventGame.State == EventGameState.Complete) return;
        
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
        var windowModel = Model as UIEventWindowModel;

        timerLabel.Text = windowModel.TimerText(eventGame.TimeController);
    }
}
