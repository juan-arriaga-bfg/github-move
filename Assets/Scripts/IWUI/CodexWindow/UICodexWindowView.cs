using UnityEngine;
using System.Collections;

public class UICodexWindowView : UIGenericPopupWindowView
{
    [SerializeField] private TabGroup tabGroup;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        UICodexWindowModel model = Model as UICodexWindowModel;
        
        tabGroup.ActivateTab(model.ActiveTabIndex);
        
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        UICodexWindowModel model = Model as UICodexWindowModel;
        
    }
}
