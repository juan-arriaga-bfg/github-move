using System;
using System.Collections.Generic;
using UnityEngine;

public class UILauncherWindowView : IWUIWindowView
{
    [IWUIBinding("#ProgressBar")] private UINineSpriteProgressBar progressBar;
    [IWUIBinding("#ProgressText")] private NSText progressText;

    private bool isGameLoaded;
    
    private AsyncInitManager asyncInitManager;

    private float progressBarStartValue = -1;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        UILauncherWindowModel windowModel = Model as UILauncherWindowModel;

        asyncInitManager = AsyncInitService.Current;

        isGameLoaded = false;
        progressBarStartValue = -1;

        progressBar.UpdateProgress(0, false);
        progressText.Text = "";
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        UILauncherWindowModel windowModel = Model as UILauncherWindowModel;
    }

    private void Update()
    {
        if (isGameLoaded)
        {
            return;
        }

        float progress = GetCurrentProgress();

        progress = Mathf.Clamp(progress, 0f, 1f);

        if (progressBarStartValue < 0)
        {
            progressBarStartValue = progress;
        }

        float value;
        
        float virtualLen = 1 - progressBarStartValue;

        if (virtualLen < 0 || Math.Abs(virtualLen) > Mathf.Epsilon) // handle div by zero and/or case when we try to show bar after loading already completed
        {
            float virtualProgress = progress - progressBarStartValue;
            value = virtualProgress / virtualLen;

            value = Mathf.Clamp(value, 0f, 1f);
        }
        else
        {
            value = 1;
        }

        progressBar.UpdateProgress(value, false);
        progressText.Text = $"{(int)(value * 100)}%";
        
        if ((int) progress >= 1)
        {
            isGameLoaded = true;
            // Controller.CloseCurrentWindow();
        }
    }

    private float GetCurrentProgress()
    {
        if (asyncInitManager == null)
        {
            return 0;
        }

        var progress = asyncInitManager.GetTotalProgress();
        return progress;
    }
}