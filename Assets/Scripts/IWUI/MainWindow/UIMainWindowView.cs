using UnityEngine;
using System.Collections;

public class UIMainWindowView : IWUIWindowView 
{
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        UIMainWindowModel windowModel = Model as UIMainWindowModel;
        
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        UIMainWindowModel windowModel = Model as UIMainWindowModel;
        
    }

    public void ShowSample()
    {
//        // get model for window
//        var model = UIService.Get.GetCachedModel<UISampleWindowModel>(UIWindowType.SampleWindow);
//        // modify model properties
//        model.RandomNumber = UnityEngine.Random.Range(0, 100);
//        // show window
//        UIService.Get.ShowWindow(UIWindowType.SampleWindow);
        
        UIService.Get.ShowWindow(UIWindowType.BankWindow);
    }
}
