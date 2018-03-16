using UnityEngine;
using System.Collections;

public class UIMessageWindowController : IWWindowController {

    public override IWWindowModel CreateModel()
    {
        UIMessageWindowModel windowModel = new UIMessageWindowModel();
        
        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }

    public static void CreateNotImplementedMessage()
    {
        var model = UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);
        
        model.Title = "Message";
        model.Message = "Not Implemented";
        model.AcceptLabel = "Ok";
        
        model.OnAccept = () => {};
        model.OnCancel = null;
        
        UIService.Get.ShowWindow(UIWindowType.MessageWindow);
    }
    
    public static void CreateDefaultMessage(string message)
    {
        var model = UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);
        
        model.Title = "Message";
        model.Message = message;
        model.AcceptLabel = "Ok";
        
        model.OnAccept = () => {};
        model.OnCancel = null;
        
        UIService.Get.ShowWindow(UIWindowType.MessageWindow);
    }
}