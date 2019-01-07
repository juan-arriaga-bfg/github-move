using DG.Tweening;

/// <summary>
/// Just listener, no actual init logic here
/// </summary>
public class UIInitProgressListenerComponent : AsyncInitItemBase
{
    private IWUIManager uiManager;

    public override bool IsCompleted => uiManager != null && uiManager.IsComplete;
    
    private float progress;
    
    public override float Progress
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
    
    public override void Execute()
    {
        DOTween.Sequence()
               .SetId(this)
               .AppendInterval(0.1f)
               .AppendCallback(() =>
                {
                    if (IsCompleted)
                    {
                        DOTween.Kill(this);
                        OnComplete(this);
                    }
                })
               .SetLoops(-1);
    }
}