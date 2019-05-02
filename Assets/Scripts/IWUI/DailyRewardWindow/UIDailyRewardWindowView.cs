using System.Collections.Generic;

public class UIDailyRewardWindowView : UIGenericPopupWindowView 
{
    [IWUIBinding("#Content")] private UIContainerViewController content;
    
    [IWUIBinding("#CloseMaskLeft")] private UIButtonViewController btnMaskLeft;
    [IWUIBinding("#CloseMaskRight")] private UIButtonViewController btnMaskRight;
    
    public override void OnViewShow()
    {
        base.OnViewShow();

        btnClose.gameObject.SetActive(false);
        
        var windowModel = Model as UIDailyRewardWindowModel;
        
        SetTitle(windowModel.Title);
        SetMessage(windowModel.Message);
        
        Fill(UpdateEntities(windowModel.Defs), content);
        content.GetScrollRect().horizontalNormalizedPosition = 0f;
    }
    
    public override void OnViewShowCompleted()
    {
        base.OnViewShowCompleted();
        
        InitButtonBase(btnMaskLeft, Controller.CloseCurrentWindow);
        InitButtonBase(btnMaskRight, Controller.CloseCurrentWindow);
    }
    
    protected virtual List<IUIContainerElementEntity> UpdateEntities(List<DailyRewardDef> entities)
    {
        var views = new List<IUIContainerElementEntity>(entities.Count);
        
        for (var i = 0; i < entities.Count; i++)
        {
            var def = entities[i];
            
            var entity = new UIDailyRewardElementEntity
            {
                ContentId = def.Icon,
                LabelText = string.Format(LocalizationService.Get("window.dailyReward.item.day", "window.dailyReward.item.day {0}"), i + 1),
                State = i == 2 ? DailyRewardState.Current : ( i < 2 ? DailyRewardState.Claimed : DailyRewardState.Lock),
                Rewards = def.Rewards,
                OnSelectEvent = null,
                OnDeselectEvent = null
            };
            
            views.Add(entity);
        }
        
        return views;
    }
}
