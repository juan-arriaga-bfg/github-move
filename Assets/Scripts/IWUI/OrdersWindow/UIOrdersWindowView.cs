using UnityEngine;
using System.Collections;

public class UIOrdersWindowView : IWUIWindowView 
{
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        UIOrdersWindowModel windowModel = Model as UIOrdersWindowModel;
        
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        UIOrdersWindowModel windowModel = Model as UIOrdersWindowModel;
        
    }
}
