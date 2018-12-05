using UnityEngine;
using System.Collections;

public class UISuperMatchHintWindowView : UIGenericPopupWindowView 
{
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        UISuperMatchHintWindowModel windowModel = Model as UISuperMatchHintWindowModel;
        
        SetTitle(windowModel.Title);
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        UISuperMatchHintWindowModel windowModel = Model as UISuperMatchHintWindowModel;
        
    }
}
