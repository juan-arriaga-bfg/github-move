using System.Collections.Generic;
using UnityEngine;

public class UILauncherWindowView : IWUIWindowView
{
    [IWUIBinding("#ProgressBar")] private UINineSpriteProgressBar progressBar;
    [IWUIBinding("#ProgressText")] private NSText progressText;

    private bool isGameLoaded;
    
    private AsyncInitManager asyncInitManager;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        UILauncherWindowModel windowModel = Model as UILauncherWindowModel;

        asyncInitManager = AsyncInitService.Current;
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
        
        if (asyncInitManager == null)
        {
            return;
        }

        var progress = asyncInitManager.GetTotalProgress();

        progress = Mathf.Clamp(progress, 0.05f, 1f);
        progressBar.UpdateProgress(progress, false);
        progressText.Text = $"{(int)(progress * 100)}%";

        if ((int) progress == 1)
        {
            isGameLoaded = true;
            // Controller.CloseCurrentWindow();
        }
    }
}