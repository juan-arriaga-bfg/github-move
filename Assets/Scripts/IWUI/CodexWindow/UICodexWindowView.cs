using UnityEngine;
using System.Collections;

public class UICodexWindowView : IWUIWindowView 
{
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        UICodexWindowModel windowModel = Model as UICodexWindowModel;
        
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        UICodexWindowModel windowModel = Model as UICodexWindowModel;
        
    }
}
