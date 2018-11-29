using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIConfirmRemoverMessageWindowView : UIMessageWindowView
{
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        UIConfirmRemoverMessageWindowModel windowModel = Model as UIConfirmRemoverMessageWindowModel;
        
        message.gameObject.SetActive(true);
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        UIConfirmRemoverMessageWindowModel windowModel = Model as UIConfirmRemoverMessageWindowModel;
        
    }
}
