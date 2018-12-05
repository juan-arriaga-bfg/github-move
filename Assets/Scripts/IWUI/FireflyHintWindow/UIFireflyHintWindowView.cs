using UnityEngine;
using System.Collections;

public class UIFireflyHintWindowView : UIGenericPopupWindowView 
{
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        UIFireflyHintWindowModel windowModel = Model as UIFireflyHintWindowModel;
       
        SetTitle(windowModel.Title);
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        UIFireflyHintWindowModel windowModel = Model as UIFireflyHintWindowModel;
        
    }
}
