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
        
        model.Title = LocalizationService.Get("common.message.message", "common.message.message");
        model.Message = LocalizationService.Get("common.message.notImplemented", "common.message.notImplementedd");
        model.AcceptLabel = LocalizationService.Get("common.button.ok", "common.button.ok");
        
        model.OnAccept = () => {};
        model.OnCancel = null;
        
        UIService.Get.ShowWindow(UIWindowType.MessageWindow);
    }
    
    public static void CreateDefaultMessage(string message, Action OnAccept = null)
    {
        var model = UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);
        
        model.Title = LocalizationService.Get("common.message.message", "common.message.message");
        model.Message = message;
        model.AcceptLabel = LocalizationService.Get("common.button.ok", "common.button.ok");
        
        model.OnAccept = OnAccept ?? (() => {});
        model.OnCancel = null;
        
        UIService.Get.ShowWindow(UIWindowType.MessageWindow);
    }

    public static void CreateMessage(string title, string message, Action OnAccept = null, Action OnCancel = null, bool isHardAccept = false)
    {
        var model = UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);
        
        model.Title = title;
        model.Message = message;
        model.AcceptLabel = LocalizationService.Get("common.button.ok", "common.button.ok");
        model.isHardAccept = isHardAccept;
        
        model.OnAccept = OnAccept ?? (() => {});
        model.OnCancel = OnCancel;
        
        UIService.Get.ShowWindow(UIWindowType.MessageWindow);
    }

    public static void CreateImageMessage(string title, string image, Action onAccept)
    {
        var model = UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);
        
        model.Title = title;
        model.Message = null;
        model.Image = image;
        model.AcceptLabel = LocalizationService.Get("common.button.ok", "common.button.ok");
        
        model.OnAccept = onAccept;
        model.OnCancel = null;
        
        UIService.Get.ShowWindow(UIWindowType.MessageWindow);
    }
    
    public static void CreateNeedCoinsMessage()
    {
        var model = UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);
        
        model.Title = string.Format(LocalizationService.Get("common.message.need", "common.message.need {0}?"), Currency.Coins.Name.ToLower());
        model.Message = null;
        model.Image = "tutorial_TextBlock_1";
        model.AcceptLabel = LocalizationService.Get("common.button.ok", "common.button.ok");
        
        model.OnAccept = () => {};
        model.OnCancel = null;
        
        UIService.Get.ShowWindow(UIWindowType.MessageWindow);
    }
    
    public static void CreateNeedCurrencyMessage(string currency)
    {
        var model = UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);
        
        model.Title = string.Format(LocalizationService.Get("common.message.need", "common.message.need {0}?"), currency.ToLower());
        model.Message = string.Format(LocalizationService.Get("common.message.notHave", "common.message.notHave {0}!"), $"<sprite name={currency}>");
        model.AcceptLabel = LocalizationService.Get("common.button.ok", "common.button.ok");
        
        model.OnAccept = () => {};
        model.OnCancel = null;
        
        UIService.Get.ShowWindow(UIWindowType.MessageWindow);
    }

    public static void CreateTimerCompleteMessage(string message, TimerComponent timer)
    {
        if(timer.Delay - timer.StartTime.GetTime().TotalSeconds < 1) return;

        var model = UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);
        
        model.Title = LocalizationService.Get("window.timerComplete.title", "window.timerComplete.title");
        model.Message = message;
        model.AcceptLabel = "";
        model.isBuy = true;
        
        model.OnAccept = timer.FastComplete;
        model.OnCancel = null;

        model.Timer = timer;
        
        UIService.Get.ShowWindow(UIWindowType.MessageWindow);
    }
}