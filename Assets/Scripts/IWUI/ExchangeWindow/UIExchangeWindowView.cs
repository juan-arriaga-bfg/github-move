using System.Collections.Generic;

public class UIExchangeWindowView : UIGenericPopupWindowView
{
    [IWUIBinding("#BuyButtonLabel")] private NSText buttonBuyLabel;
    
    [IWUIBinding("#Content")] private UIContainerViewController content;
    [IWUIBinding("#BuyButton")] private UIButtonViewController btnBuy;
    
    private bool isClick;
    
    public override void InitView(IWWindowModel model, IWWindowController controller)
    {
        base.InitView(model, controller);

        btnBuy.Init()
            .ToState(GenericButtonState.Active)
            .OnClick(OnBuyClick);
    }
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        UIExchangeWindowModel windowModel = Model as UIExchangeWindowModel;
        
        SetTitle(windowModel.Title);
        SetMessage(windowModel.Message);

        buttonBuyLabel.Text = windowModel.Button;

        isClick = false;
        
        Fill(UpdateEntities(windowModel.Products), content);
    }
    
    private List<IUIContainerElementEntity> UpdateEntities(List<CurrencyPair> entities)
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
    
    public override void OnViewCloseCompleted()
    {
        base.OnViewCloseCompleted();
        
        if(isClick == false) return;
        
        UIExchangeWindowModel windowModel = Model as UIExchangeWindowModel;
        
        CurrencyHellper.Purchase(windowModel.Products, windowModel.Price, success =>
        {
            if(success == false) return;
            
            windowModel.OnClick?.Invoke();
        });
    }

    private void OnBuyClick()
    {
        if(isClick) return;
        
        UIExchangeWindowModel windowModel = Model as UIExchangeWindowModel;
        
        if(CurrencyHellper.IsCanPurchase(windowModel.Price, true) == false) return;

        isClick = true;
        
        Controller.CloseCurrentWindow();
    }
}
