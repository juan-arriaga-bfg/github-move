using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UIDailyRewardWindowView : UIGenericPopupWindowView 
{
    [IWUIBinding("#Content")] private UIContainerViewController content;
    
    [IWUIBinding("#CloseMaskLeft")] private UIButtonViewController btnMaskLeft;
    [IWUIBinding("#CloseMaskRight")] private UIButtonViewController btnMaskRight;
    
    public override void OnViewShow()
    {
        base.OnViewShow();

        Controller.Window.IgnoreBackButton = true;
        
        btnClose.gameObject.SetActive(false);
        
        var model = Model as UIDailyRewardWindowModel;
        
        SetTitle(model.Title);
        SetMessage(model.Message);
        
        Fill(UpdateEntities(model.Defs), content);
        
        content.GetScrollRect().horizontalNormalizedPosition = 0f;
    }
    
    private void Scroll(int index)
    {
        DOTween.Kill(content);
        if (content.Tabs.size == index + 1)
        {
            index++;
        }
        
        var posX = Mathf.Clamp((-content.CachedRectTransform.sizeDelta.x / content.Tabs.size) * index, -content.CachedRectTransform.sizeDelta.x, 0);

        if (Mathf.Abs(content.CachedRectTransform.anchoredPosition.x - posX) <= 0.01f) return;

        const float duration = 1.5f;
        var percent = Mathf.Abs(content.CachedRectTransform.anchoredPosition.x / content.CachedRectTransform.sizeDelta.x);

        content.GetScrollRect().enabled = false;
        content.CachedRectTransform.DOAnchorPosX(posX, duration*percent + duration*(1-percent))
               .SetEase(Ease.InOutBack)
               .SetId(content)
               .OnComplete(() => { content.GetScrollRect().enabled = true; });
    }

    public override void OnViewShowCompleted()
    {
        base.OnViewShowCompleted();

        // Do not allow to close window by back click 
        // InitButtonBase(btnMaskLeft, Controller.CloseCurrentWindow);
        // InitButtonBase(btnMaskRight, Controller.CloseCurrentWindow);
        if (btnBackLayer != null)
        {
            btnBackLayer.ToState(GenericButtonState.UnActive).OnClick(() => { });
        }
        
        var model = Model as UIDailyRewardWindowModel;
        
        Scroll(model.Day);
    }
    
    protected virtual List<IUIContainerElementEntity> UpdateEntities(List<DailyRewardDef> entities)
    {
        var model = Model as UIDailyRewardWindowModel;
        
        var views = new List<IUIContainerElementEntity>(entities.Count);
        
        for (var i = 0; i < entities.Count; i++)
        {
            var def = entities[i];
            
            var entity = new UIDailyRewardElementEntity
            {
                ContentId = def.Icon,
                LabelText = string.Format(LocalizationService.Get("window.dailyReward.item.day", "window.dailyReward.item.day {0}"), i + 1), 

                State = i == model.Day 
                    ? DailyRewardState.Current 
                    : i < model.Day 
                        ? DailyRewardState.Claimed 
                        : DailyRewardState.Lock,

                Rewards = def.Rewards,
                OnSelectEvent = null,
                OnDeselectEvent = null
            };

            views.Add(entity);
        }
        
        return views;
    }
}
