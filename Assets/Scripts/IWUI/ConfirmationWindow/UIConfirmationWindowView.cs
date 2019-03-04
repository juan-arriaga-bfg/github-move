using UnityEngine;

public class UIConfirmationWindowView : UIGenericPopupWindowView 
{
    [IWUIBinding("#ElementMarket")] private GameObject elementMarket;
    [IWUIBinding("#ElementShop")] private GameObject elementShop;
    
    [IWUIBinding("#AnchorMarket")] private Transform anchorMarket;
    [IWUIBinding("#AnchorShop")] private Transform anchorShop;
    
    [IWUIBinding("#ButtonBuy")] private UIBaseButtonViewController buttonBuy;
    
    [IWUIBinding("#ButtonBuyLabel")] private NSText buttonBuyLabel;
    
    [IWUIBinding("#AmountMarket")] private NSText amountMarket;
    [IWUIBinding("#AmountShop")] private NSText amountShop;

    [IWUIBinding("#NameLabel")] private NSText nameLabelMarket;
    
    private bool isClick;
    private Transform icon;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UIConfirmationWindowModel;
        
        SetTitle(windowModel.Title);
        SetMessage(windowModel.Message);

        buttonBuyLabel.Text = windowModel.ButtonText;
        amountMarket.Text = windowModel.ProductAmountText;
        nameLabelMarket.Text = windowModel.ProductNameText;
        
        elementMarket.SetActive(true);
        elementShop.SetActive(false);
        
        CreateIcon(anchorMarket, windowModel.Icon);
        
        isClick = false;
    }
    
    private void CreateIcon(Transform parent, string id)
    {
        icon = UIService.Get.PoolContainer.Create<Transform>((GameObject) ContentService.Current.GetObjectByName(id));
        icon.SetParentAndReset(parent);
    }
    
    public override void OnViewShowCompleted()
    {
        base.OnViewShowCompleted();
		
        buttonBuy
            .ToState(GenericButtonState.Active)
            .OnClick(OnClick);
    }

    public override void OnViewCloseCompleted()
    {
        if(icon != null) UIService.Get.PoolContainer.Return(icon.gameObject);
        
        base.OnViewCloseCompleted();
        
        var windowModel = Model as UIConfirmationWindowModel;
        var action = isClick ? windowModel.OnAccept : windowModel.OnCancel;

        windowModel.OnAccept = null;
        windowModel.OnCancel = null;
        windowModel.OnAcceptTap = null;
        
        action?.Invoke();
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        var windowModel = Model as UIConfirmationWindowModel;
        if (isClick) windowModel.OnAcceptTap?.Invoke();
    }

    private void OnClick()
    {
        if(isClick) return;

        isClick = true;
        Controller.CloseCurrentWindow();
    }
}