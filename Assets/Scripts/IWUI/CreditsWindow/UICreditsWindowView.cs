using DG.Tweening;
using UnityEditor.Sprites;
using UnityEngine;
using UnityEngine.UI;

public class UICreditsWindowView : UIGenericPopupWindowView
{
    [IWUIBinding("#Team")] private NSText team;
    [IWUIBinding("#BigFish")] private NSText bigFish;
    [IWUIBinding("#Trademark")] private NSText trademark;
    [IWUIBinding("#Scroll View")] private ScrollRect scroll;
    
    [IWUIBinding("#Content")] private RectTransform parrent;
    [IWUIBinding("#PauseButton")] private UIButtonViewController pauseButton;

    private const float delay = 80;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UICreditsWindowModel;
        
        SetTitle(windowModel.Title);
        
        team.Text = "";

        var iconManager = IconService.Instance.Manager as IconResourceManager;

        foreach (var iconData in iconManager.CachedIconsData.Keys)
        {
            team.Text += $"{iconData}: text for <sprite name={iconData}> example \n";
            team.Text += $"<sprite name={iconData}> and icon on begining string and end string <sprite name={iconData}> \n";
            team.Text += $"and simple long long lorem ipsum text for debug \n\n";
            
        }
        
//        bigFish.Text = windowModel.BigFish;
//        trademark.Text = windowModel.Trademark;
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(parrent);
        
        scroll.verticalNormalizedPosition = 1;
        scroll.enabled = false;
    }

    public override void OnViewShowCompleted()
    {
        base.OnViewShowCompleted();

        StartAnimation();
        
        pauseButton.ToState(GenericButtonState.Active)
            .OnDown(OnDown)
            .OnUp(OnUp);
    }

    private void StartAnimation()
    {
        return;
        DOTween.Kill(parrent);
        
        parrent.DOAnchorPosY(parrent.sizeDelta.y, delay)
            .SetId(parrent)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                scroll.verticalNormalizedPosition = 1;
                StartAnimation();
            });
    }
    
    public override void OnViewClose()
    {
        base.OnViewClose();

        DOTween.Kill(parrent);
    }
    
    private void OnDown()
    {
        DOTween.Kill(parrent);
        scroll.enabled = true;
    }
    
    private void OnUp()
    {
        return;
        scroll.enabled = false;

        var time = delay * (1 - parrent.anchoredPosition.y / parrent.sizeDelta.y);
        
        parrent.DOAnchorPosY(parrent.sizeDelta.y, time)
            .SetId(parrent)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                scroll.verticalNormalizedPosition = 1;
                StartAnimation();
            });
    }
}
