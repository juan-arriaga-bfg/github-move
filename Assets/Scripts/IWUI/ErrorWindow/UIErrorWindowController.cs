using System;
using BfgAnalytics;
using UnityEngine;

public class UIErrorWindowController : IWWindowController
{

    private static DateTime? freeSpaceAnalyticsSendTimestamp;
    
    public override IWWindowModel CreateModel()
    {
        UIErrorWindowModel windowModel = new UIErrorWindowModel();
        
        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }

    public static void AddNoFreeSpaceError()
    {
        AddError(LocalizationService.Get("message.error.freeSpace", "message.error.freeSpace"));
        
        DateTime now = DateTime.UtcNow;
        if (freeSpaceAnalyticsSendTimestamp == null || Math.Abs((now - freeSpaceAnalyticsSendTimestamp.Value).TotalSeconds) > 30)
        {
            freeSpaceAnalyticsSendTimestamp = now;
            Analytics.SendNoSpace();
        }
    }
    
    public static void AddError(string message)
    {
        var model = UIService.Get.GetCachedModel<UIErrorWindowModel>(UIWindowType.ErrorWindow);
        
        model.Messages.Add(message);

        var window = UIService.Get.GetShowedWindowByName(UIWindowType.ErrorWindow);

        if (window == null)
        {
            UIService.Get.ShowWindow(UIWindowType.ErrorWindow);
            return;
        }
        
        var view = window.CurrentView as UIErrorWindowView;
        view.Next();
    }
}
