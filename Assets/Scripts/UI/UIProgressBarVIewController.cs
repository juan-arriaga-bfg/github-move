using UnityEngine;
using DG.Tweening;

public class UIProgressBarVIewController : IWUIWindowViewController
{
    [SerializeField] private NSText progressLabel;

    [SerializeField] private RectTransform progressBarRect;

    public override void OnViewShow(IWUIWindowView context)
    {
        base.OnViewShow(context);
        
        if (progressBarRect != null) progressBarRect.sizeDelta = new Vector2(0f, progressBarRect.sizeDelta.y);
    }

    public virtual void SetProgress(int currentAmount, int targetAmount, bool isAnimate = true)
    {
        var percent = currentAmount / (float) targetAmount;

        if (progressLabel != null) progressLabel.Text = $"{currentAmount}/{targetAmount}";
        
        if (progressBarRect == null) return;
        
        var parentRect = progressBarRect.parent.GetComponent<RectTransform>();
        
        if (parentRect == null) return;

        var targetProgressSize = new Vector2( Mathf.Lerp(0f, parentRect.sizeDelta.x, percent), progressBarRect.sizeDelta.y);

        DOTween.Kill(progressBarRect);
        if (isAnimate)
        {
            var sequence = DOTween.Sequence().SetId(progressBarRect);
            sequence.Append(progressBarRect.DOSizeDelta(targetProgressSize, 0.5f));
        }
        else
        {
            progressBarRect.sizeDelta = targetProgressSize;
        }
    }
}
