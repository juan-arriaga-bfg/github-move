using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIEventWindowView : UIGenericPopupWindowView 
{
    [IWUIBinding("#Content")] private UIContainerViewController content;
    
    [IWUIBinding("#MainProgressLabel")] private NSText progressLabel;
    [IWUIBinding("#MainTimerLabel")] private NSText timerLabel;
    [IWUIBinding("#ButtonShowLabel")] private NSText btnShowLabel;
    
    [IWUIBinding("#ButtonShow")] private UIButtonViewController btnShow;
    
    [IWUIBinding("#MainProgressLine")] private RectTransform mainLine;
    [IWUIBinding("#ProgressLine")] private RectTransform secondLine;

    private AmountRange mainProgressBorder = new AmountRange(10, 145);
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UIEventWindowModel;
        
        SetTitle(windowModel.Title);
        SetMessage(windowModel.Message);

        btnShowLabel.Text = windowModel.ButtonText;
        
        if(windowModel.Timer != null) windowModel.Timer.OnTimeChanged += OnTimeChanged;
        OnTimeChanged();
        
        Fill(UpdateEntities(), content);
        
        var defs = GameDataService.Current.EventManager.Defs[EventName.OrderSoftLaunch];
        var target = defs.Sum(def => def.Prices[0].Amount);
        var current = ProfileService.Current.GetStorageItem(Currency.Token.Name).Amount;
        var progress = Mathf.Clamp(mainProgressBorder.Max * (current / (float) target) + mainProgressBorder.Min, mainProgressBorder.Min, mainProgressBorder.Max);
        
        progressLabel.Text = $"{current}/{target}";
        mainLine.sizeDelta = new Vector2(progress, mainLine.sizeDelta.y);
    }

    public override void OnViewShowCompleted()
    {
        base.OnViewShowCompleted();
        
        InitButtonBase(btnShow, OnShow);
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

    private void OnShow()
    {
        
    }
    
    private void OnTimeChanged()
    {
        var windowModel = Model as UIEventPreviewWindowModel;

//        timerLabel.Text = windowModel.TimerText;
    }
}
