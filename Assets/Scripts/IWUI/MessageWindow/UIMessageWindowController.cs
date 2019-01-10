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
        
        UIService.Get.ShowWindow(UIWindowType.MessageWindow);
    }
    
    public static void CreateDefaultMessage(string message, Action onAccept = null)
    {
        var model = UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);
        
        model.Title = LocalizationService.Get("common.message.message", "common.message.message");
        model.Message = message;
        model.AcceptLabel = LocalizationService.Get("common.button.ok", "common.button.ok");
        
        model.OnAccept = onAccept ?? (() => {});
        
        UIService.Get.ShowWindow(UIWindowType.MessageWindow);
    }

    public static void CreateMessage(string title, string message, string prefab = null, Action onAccept = null, Action onCancel = null, Action onClose = null, bool isHardAccept = false)
    {
        var model = UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);
        
        model.Title = title;
        model.Message = message;
        model.Prefab = prefab;
        model.AcceptLabel = LocalizationService.Get("common.button.ok", "common.button.ok");
        model.CancelLabel = LocalizationService.Get("common.button.no", "common.button.no");
        model.IsHardAccept = isHardAccept;
        
        model.OnAccept = onAccept ?? (() => {});
        model.OnCancel = onCancel;
        model.OnClose = onClose;
        
        UIService.Get.ShowWindow(UIWindowType.MessageWindow);
    }

    public static void CreatePrefabMessage(string title, string prefab, string message = null)
    {
        var model = UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);
        
        model.Title = title;
        model.Message = message;
        model.Prefab = prefab;
        model.AcceptLabel = LocalizationService.Get("common.button.ok", "common.button.ok");
        
        model.OnAccept = () => {};
        
        UIService.Get.ShowWindow(UIWindowType.MessageWindow);
    }
    
    public static void CreateNeedCurrencyMessage(string currency, string diff = "")
    {
        var model = UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);

        model.Prefab = currency == Currency.Mana.Name ? "NotHaveMana" : null;
        model.Title = LocalizationService.Get($"window.notHave.title.{currency.ToLower()}", $"window.notHave.title.{currency.ToLower()}?");

        model.Message = string.Format(
            LocalizationService.Get($"window.notHave.message.{currency.ToLower()}",
                $"window.notHave.message.{currency.ToLower()}" + "{0}!"),
            $"<sprite name={(currency == Currency.Mana.Name ? Currency.Mana.Icon : currency)}>{diff}");
        
        model.AcceptLabel = LocalizationService.Get("common.button.ok", "common.button.ok");
            
        model.OnAccept = () => {};
        
        UIService.Get.ShowWindow(UIWindowType.MessageWindow);
    }

    public static void CreateTimerCompleteMessage(string message, TimerComponent timer)
    {
        if(timer.Delay - timer.StartTime.GetTime().TotalSeconds < 1) return;

        var model = UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);
        
        model.Title = LocalizationService.Get("window.timerComplete.title", "window.timerComplete.title");
        model.Message = message;
        model.AcceptLabel = "";
        model.IsBuy = true;
        
        model.OnAccept = timer.FastComplete;
        model.OnCancel = null;

        model.Timer = timer;
        
        UIService.Get.ShowWindow(UIWindowType.MessageWindow);
    }
}