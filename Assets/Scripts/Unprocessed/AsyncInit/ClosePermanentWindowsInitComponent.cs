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
        foreach (var windowName in windowsToClose)
        {
            var window = IWUIManager.Instance.GetShowedWindowByName(windowName);
            window.WindowController.CloseCurrentWindow(controller => { OnWindowClosed(); });
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