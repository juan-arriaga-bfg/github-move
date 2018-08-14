using System;
using UnityEngine;

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
    
    public static void CreateDefaultMessage(string message, Action OnAccept = null)
    {
        var model = UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);
        
        model.Title = "Message";
        model.Message = message;
        model.AcceptLabel = "Ok";
        
        model.OnAccept = OnAccept ?? (() => {});
        model.OnCancel = null;
        
        UIService.Get.ShowWindow(UIWindowType.MessageWindow);
    }

    public static void CreateMessage(string title, string message, Action OnAccept = null, Action OnCancel = null, bool isHardAccept = false)
    {
        var model = UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);
        
        model.Title = title;
        model.Message = message;
        model.AcceptLabel = "Ok";
        model.isHardAccept = isHardAccept;
        
        model.OnAccept = OnAccept ?? (() => {});
        model.OnCancel = OnCancel;
        
        UIService.Get.ShowWindow(UIWindowType.MessageWindow);
    }

    public static void CreateImageMessage(string title, string image, Action onAccept)
    {
        var model= UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);
        
        model.Title = title;
        model.Message = null;
        model.Image = image;
        model.AcceptLabel = "Ok";
        
        model.OnAccept = onAccept;
        model.OnCancel = null;
        
        UIService.Get.ShowWindow(UIWindowType.MessageWindow);
    }
    
    public static void CreateNeedCoinsMessage()
    {
        var model= UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);
        
        model.Title = "Need coins?";
        model.Message = null;
        model.Image = "tutorial_TextBlock_1";
        model.AcceptLabel = "Ok";
        
        model.OnAccept = () => {};
        model.OnCancel = null;
        
        UIService.Get.ShowWindow(UIWindowType.MessageWindow);
    }
    
    public static void CreateNeedCurrencyMessage(string currency)
    {
        var model= UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);
        
        model.Title = string.Format("Need {0}?", currency.ToLower());
        model.Message = string.Format("You do not have enough <sprite name={0}>!", currency);
        model.AcceptLabel = "Ok";
        
        model.OnAccept = () => {};
        model.OnCancel = null;
        
        UIService.Get.ShowWindow(UIWindowType.MessageWindow);
    }

    public static void CreateTimerCompleteMessage(string title, string message, string buttonText, TimerComponent timer, Action onAccept)
    {
        var model= UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);
        
        model.Title = title;
        model.Message = message;
        model.AcceptLabel = buttonText;
        
        model.OnAccept = onAccept;
        model.OnCancel = null;

        model.Timer = timer;
        
        UIService.Get.ShowWindow(UIWindowType.MessageWindow);
    }
}