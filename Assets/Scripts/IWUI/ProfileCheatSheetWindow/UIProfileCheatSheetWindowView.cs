using UnityEngine;
using System.Collections;

public class UIProfileCheatSheetWindowView : IWUIWindowView 
{
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        UIProfileCheatSheetWindowModel windowModel = Model as UIProfileCheatSheetWindowModel;
        
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        UIProfileCheatSheetWindowModel windowModel = Model as UIProfileCheatSheetWindowModel;
        
    }
}
