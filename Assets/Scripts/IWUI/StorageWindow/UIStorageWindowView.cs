using UnityEngine;

public class UIStorageWindowView : UIGenericPopupWindowView
{
    [SerializeField] private NSText capacity;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UIStorageWindowModel;
        
        SetTitle(windowModel.Title);
        SetMessage(windowModel.Message);
        capacity.Text = windowModel.Capacity;
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        var windowModel = Model as UIStorageWindowModel;
    }

    public void OnClick()
    {
        
    }
}
