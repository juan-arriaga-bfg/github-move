using System.Collections.Generic;

public class UIEventWindowView : UIGenericPopupWindowView 
{
    [IWUIBinding("#Content")] private UIContainerViewController content;
    
    [IWUIBinding("#MainTimerLabel")] private NSText timerLabel;
    [IWUIBinding("#ButtonShowLabel")] private NSText btnShowLabel;
    
    [IWUIBinding("#ButtonShow")] private UIButtonViewController btnShow;
    
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
        
        for (var i = 0; i < defs.Count; i++)
        {
            var def = defs[i];
            
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
