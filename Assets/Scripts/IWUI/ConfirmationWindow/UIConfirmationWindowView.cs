using UnityEngine;

public class UIConfirmationWindowView : UIGenericPopupWindowView 
{
    [IWUIBinding("#Anchor")] private Transform anchor;
    
    [IWUIBinding("#ButtonBuy")] private UIBaseButtonViewController buttonBuy;
    
    [IWUIBinding("#ButtonBuyLabel")] private NSText buttonBuyLabel;
    
    [IWUIBinding("#Amount")] private NSText amount;

    [IWUIBinding("#NameLabel")] private NSText nameLabel;
    
    private bool isClick;
    private Transform icon;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UIConfirmationWindowModel;
        
        SetTitle(windowModel.Title);
        SetMessage(windowModel.Message);

        buttonBuyLabel.Text = windowModel.ButtonText;
        amount.Text = windowModel.ProductAmountText;
        nameLabel.Text = windowModel.ProductNameText;
        
        CreateIcon(anchor, windowModel.Icon);
        
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
        if (icon != null) UIService.Get.PoolContainer.Return(icon.gameObject);
        
        base.OnViewCloseCompleted();
        
        var windowModel = Model as UIConfirmationWindowModel;
        
        if (isClick == false) windowModel.OnCancel?.Invoke();
        
        windowModel.OnAccept = null;
        windowModel.OnCancel = null;
    }

    private void OnClick()
    {
        if(isClick) return;

        isClick = true;
        
        var windowModel = Model as UIConfirmationWindowModel;
        windowModel.OnAccept?.Invoke(buttonBuy.transform);
        
        Controller.CloseCurrentWindow();
    }
}