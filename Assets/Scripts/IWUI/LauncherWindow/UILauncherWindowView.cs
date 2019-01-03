using System.Collections.Generic;
using UnityEngine;

public class UILauncherWindowView : IWUIWindowView
{
    [IWUIBinding("#ProgressBar")] private UINineSpriteProgressBar progressBar;
    [IWUIBinding("#ProgressText")] private NSText progressText;

    private ComplexOperationProgress loadingProgress;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        UILauncherWindowModel windowModel = Model as UILauncherWindowModel;

        var initService = AsyncInitService.Current;
        
        loadingProgress = new ComplexOperationProgress();
        loadingProgress.AddItem(initService.GetAsyncInitComponent<MainSceneLoaderComponent>(), 1000);
        loadingProgress.AddItem(new UIInitializationProgressListner(), 1000);
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        UILauncherWindowModel windowModel = Model as UILauncherWindowModel;
    }

    private void Update()
    {
        if (loadingProgress == null)
        {
            return;
        }

        var progress = loadingProgress.GetTotalProgress();

        progress = Mathf.Clamp(progress, 0.05f, 1f);
        progressBar.UpdateProgress(progress, false);
        progressText.Text = $"{(int)(progress * 100)}%";
    }
}

public interface IHaveProgress
{
    float Progress { get; }
}

public class UIInitializationProgressListner : IHaveProgress
{
    private IWUIManager uiManager;

    private float progress;
    
    public float Progress
    {
        get
        {
            if (uiManager == null)
            {
                uiManager = UIService.Get;
                if (uiManager == null)
                {
                    return 0;
                }

                uiManager.OnProgressUpdate += OnProgress;
            }
            
            if (uiManager.IsComplete)
            {
                uiManager.OnProgressUpdate -= OnProgress;
                return 1;
            }

            return progress;
        }
    }

    private void OnProgress(string windowName, float progress)
    {
        this.progress = progress;
    }
}

public class ComplexOperationProgress
{
    struct ComplexOperationItem
    {
        public int Weight;
        public IHaveProgress Provider;
    }
    
    private List<ComplexOperationItem> items = new List<ComplexOperationItem>();

    public void AddItem(IHaveProgress item, int weight)
    {
        items.Add(new ComplexOperationItem
        {
            Provider = item,
            Weight = weight
        });
    }
    
    public float GetTotalProgress()
    {
        float total = 0;
        float progress = 0;
        foreach (var item in items)
        {
            total += item.Weight;
            progress += (item.Provider.Progress * item.Weight);

            if ((int) (item.Provider.Progress) < 1)
            {
                Debug.Log($"[ComplexOperationProgress] => Component progress: [{item.Provider.GetType()}]: {(int)(item.Provider.Progress * 100)}%");
            }
        }

        float totalProgress = progress / total;

        Debug.Log($"[ComplexOperationProgress] => Total progress: {totalProgress}%");
        
        return totalProgress;
    }
}
