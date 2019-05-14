using System.Collections.Generic;

public class UISaveWindowView : UIGenericPopupWindowView 
{
    [IWUIBinding("#Content")] private UIContainerViewController content;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UISaveWindowModel;
        
        SetTitle(windowModel.Title);
        SetMessage(windowModel.Message);
        
        Fill(new List<IUIContainerElementEntity>(windowModel.Profiles), content);
    }
}