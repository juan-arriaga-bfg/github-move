using UnityEngine;
using System.Collections;

public class UITavernWindowView : IWUIWindowView 
{
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        UITavernWindowModel windowModel = Model as UITavernWindowModel;
        
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        UITavernWindowModel windowModel = Model as UITavernWindowModel;
        
    }
}
