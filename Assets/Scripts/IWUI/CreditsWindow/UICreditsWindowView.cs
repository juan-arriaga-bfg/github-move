using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UICreditsWindowView : UIGenericPopupWindowView 
{
    [IWUIBinding("#Team")] private NSText team;
    [IWUIBinding("#BigFish")] private NSText bigFish;
    [IWUIBinding("#Trademark")] private NSText trademark;
    [IWUIBinding("#Scroll View")] private ScrollRect scroll;
    
    [IWUIBinding("#Content")] private RectTransform parrent;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UICreditsWindowModel;
        
        SetTitle(windowModel.Title);
        
        team.Text = windowModel.Team;
        bigFish.Text = windowModel.BigFish;
        trademark.Text = windowModel.Trademark;
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(parrent);
        
        scroll.verticalNormalizedPosition = 1;
        scroll.enabled = false;
    }

    public override void OnViewShowCompleted()
    {
        base.OnViewShowCompleted();
        
        parrent.DOAnchorPosY(parrent.sizeDelta.y, 80f)
            .SetId(parrent)
            .SetEase(Ease.Linear)
            .SetLoops(int.MaxValue)
            .OnComplete(() => { scroll.verticalNormalizedPosition = 1; });
    }

    public override void OnViewClose()
    {
        base.OnViewClose();

        DOTween.Kill(parrent);
    }
}
