using System.Collections.Generic;
using BfgAnalytics;

public class UIEventAlmostWindowView : UIGenericPopupWindowView 
{
    [IWUIBinding("#Content")] private UIContainerViewController content;
    
    [IWUIBinding("#Message2")] private NSText message2;
    [IWUIBinding("#Mark")] private NSText markLabel;
    [IWUIBinding("#ButtonBuyLabel")] private NSText btnBuyLabel;
    
    [IWUIBinding("#ButtonBuy")] private UIButtonViewController btnBuy;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UIEventAlmostWindowModel;
        
        SetTitle(windowModel.Title);
        SetMessage(windowModel.Message);
        
        message2.Text = windowModel.Message2;
        markLabel.Text = windowModel.MarkText;
        btnBuyLabel.Text = windowModel.ButtonText;
        
        Fill(UpdateEntities(), content);
    }

    public override void OnViewShowCompleted()
    {
        base.OnViewShowCompleted();
        
        InitButtonBase(btnBuy, OnClick);
    }

    public override void OnViewClose()
    {
        var windowModel = Model as UIEventAlmostWindowModel;

        windowModel.Price = null;
        windowModel.Rewards = null;
        
        base.OnViewClose();
    }

    private void OnClick()
    {
        var windowModel = Model as UIEventAlmostWindowModel;
        
        var flyPosition = GetCanvas().worldCamera.WorldToScreenPoint(btnBuy.transform.position);
        
        CurrencyHelper.PurchaseAndProvideSpawn(windowModel.Rewards, windowModel.Price, null, flyPosition, null, false, true);
        Analytics.SendPurchase("screen_event_offer", $"stage{windowModel.Step}", new List<CurrencyPair>{windowModel.Price}, new List<CurrencyPair>(windowModel.Rewards), false, false);
        
        Controller.CloseCurrentWindow();
    }
    
    private List<IUIContainerElementEntity> UpdateEntities()
    {
        var windowModel = Model as UIEventAlmostWindowModel;
        var views = new List<IUIContainerElementEntity>(windowModel.Rewards.Count);
        
        foreach (var def in windowModel.Rewards)
        {
            var entity = new UISimpleScrollElementEntity
            {
                ContentId = def.Currency,
                LabelText = $"x{def.Amount}",
                OnSelectEvent = null,
                OnDeselectEvent = null
            };
            
            views.Add(entity);
        }
        
        return views;
    }
}
