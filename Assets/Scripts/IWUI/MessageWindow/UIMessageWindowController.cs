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
        model.Message = LocalizationService.Get("common.message.notImplemented", "common.message.notImplemented");
        model.AcceptLabel = LocalizationService.Get("common.button.ok", "common.button.ok");
        
        model.OnAccept = () => {};
        
        UIService.Get.ShowWindow(UIWindowType.MessageWindow);
    }

    public static void CreateNoInternetMessage()
    {
        var model = UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);

        model.Title = LocalizationService.Get("common.message.no.internet.title",     "common.message.no.internet.title");
        model.Message = LocalizationService.Get("common.message.no.internet.message", "common.message.no.internet.message");
        model.AcceptLabel = LocalizationService.Get("common.button.ok",               "common.button.ok");

        model.OnAccept = () => { };

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

    public static void CreateMessageWithTwoButtons(string title, string message, string acceptText = null, string cancelText = null, Action onAccept = null, Action onCancel = null, Action onClose = null)
    {
        var model = UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);
        
        model.Title = title;
        model.Message = message;
        model.AcceptLabel = string.IsNullOrEmpty(acceptText) ? LocalizationService.Get("common.button.ok", "common.button.ok") : acceptText;
        model.CancelLabel = string.IsNullOrEmpty(cancelText) ? LocalizationService.Get("common.button.no", "common.button.no") : cancelText;
        
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

    public static void CreateTimerCompleteMessage(string message, string analyticsLocation, TimerComponent timer, Action onCancel = null)
    {
        if(timer.Delay - timer.StartTime.GetTime().TotalSeconds < 1) return;

        var model = UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);
        
        model.Title = LocalizationService.Get("window.timerComplete.title", "window.timerComplete.title");
        model.Message = message;
        model.AcceptLabel = "";
        model.CancelLabel = LocalizationService.Get("common.button.stop", "common.button.stop");
        
        model.IsBuy = true;

        model.OnAccept = () => { timer.FastComplete(analyticsLocation); };
        model.OnCancel = onCancel;
        
        model.Timer = timer;
        
        UIService.Get.ShowWindow(UIWindowType.MessageWindow);
    }
    
    public static void CreateQuitMessage()
    {
        var model = UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);
        
        model.Title = LocalizationService.Get("window.quit.title", "window.quit.title");
        model.Message = LocalizationService.Get("window.quit.message", "window.quit.message");
        model.AcceptLabel = LocalizationService.Get("window.quit.accept", "window.quit.accept");
        model.CancelLabel = LocalizationService.Get("window.quit.cancel", "window.quit.cancel");
        
        model.IsBuy = false;
        
        model.OnAccept = () =>
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        };
        model.OnCancel = () => {};
        
        UIService.Get.ShowWindow(UIWindowType.MessageWindow);
    }

    public static void CreateVersionMessage()
    {
        var model = UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);
        
        model.Title = LocalizationService.Get("window.version.title", "window.version.title");

        model.Message += $"{LocalizationService.Get("window.version.game", "window.version.game")}: {IWProjectVersionSettings.Instance.CurrentVersion}\n";
        
        // todo: hide on PROD
        model.Message += $"Server: {NetworkUtils.Instance.GetHostUrl()}\n";

        model.OnAccept = null;
        model.OnCancel = null;

        UIService.Get.ShowWindow(UIWindowType.MessageWindow);
    }

    public static void CreateProgressOverrideMessage(bool isLocal, Action onOverride, Action onCancel = null)
    {
        var model = UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);
        var messageKey = $"window.progress.override.message.{(isLocal ? "device" : "server")}";
        
        model.Title = LocalizationService.Get("window.progress.override.title", "window.progress.override.title");
        model.Message = LocalizationService.Get(messageKey, messageKey);
        model.Prefab = isLocal ? "LocalToServer" : "ServerToLocal";
        
        model.AcceptLabel = LocalizationService.Get("common.button.cancel", "common.button.cancel");
        model.CancelLabel = LocalizationService.Get("common.button.override", "common.button.override");
        
        model.OnAccept = onCancel ?? (() => {});
        model.OnClose = onCancel ?? (() => {});
        model.OnCancel = onOverride;

        UIService.Get.ShowWindow(UIWindowType.MessageWindow);
    }
}