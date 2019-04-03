using System.Collections.Generic;
using UnityEngine;

public class ClosePermanentWindowsInitComponent : AsyncInitComponentBase
{
    private int remainingWindows;
    
    private readonly List<string> windowsToClose = new List<string>
    {
        UIWindowType.MainWindow,
        UIWindowType.ResourcePanelWindow,
        UIWindowType.WaitWindow,
        UIWindowType.SettingsWindow
    };
    
    public override void Execute()
    {
        remainingWindows = windowsToClose.Count;
        for (var i = 0; i < windowsToClose.Count; i++)
        {
            var windowName = windowsToClose[i];
            var window = IWUIManager.Instance.GetShowedWindowByName(windowName);
            if (window == null)
            {
                IW.Logger.Log($"[ClosePermanentWindowsInitComponent] => Execute: [{i+1}/{windowsToClose.Count}] - {windowName} Already closed");
                remainingWindows--;
                continue;
            }

            IW.Logger.Log($"[ClosePermanentWindowsInitComponent] => Execute: [{i+1}/{windowsToClose.Count}] - {windowName} Closing...");

            window.WindowController.CloseCurrentWindow(controller =>
            {
                OnWindowClosed();
            });
        }
    }

    private void OnWindowClosed()
    {
        remainingWindows--;
        
        if (remainingWindows == 0)
        {
            isCompleted = true;
            OnComplete(this);
        }
    }
}