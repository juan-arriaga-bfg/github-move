using UnityEngine;
using System.Collections;

public class UILauncherWindowView : IWUIWindowView 
{
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        UILauncherWindowModel windowModel = Model as UILauncherWindowModel;
        
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        UILauncherWindowModel windowModel = Model as UILauncherWindowModel;
        
    }
}
