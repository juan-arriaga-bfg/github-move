using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIChestMessageWindowView : UIGenericPopupWindowView
{
    [IWUIBinding("#Delimiter")] private NSText delimiter;
    [IWUIBinding("#OpenButtonLabel")] private NSText btnOpenLabel;
    
    [IWUIBinding("#ChestAnchor")] private Transform chestAnchor;
    [IWUIBinding("#Content")] private UIContainerViewController content;
    [IWUIBinding("#OpenButton")] private UIButtonViewController btnOpen;
    
    private Transform chest;
    
    private bool isOpen;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UIChestMessageWindowModel;
        
        isOpen = false;
        
        SetTitle(windowModel.Title);
        SetMessage(windowModel.Message);
        
        btnOpenLabel.Text = windowModel.ButtonText;
        delimiter.Text = windowModel.DelimiterText;

        CreateIcon(chestAnchor, windowModel.ChestComponent.Def.Uid);
        Fill(UpdateEntities(windowModel.Icons()), content);
        
        var scrollRect = content.GetScrollRect();
        
        if (scrollRect != null) scrollRect.horizontalNormalizedPosition = 0f;
    }
    
    private void CreateIcon(Transform parent, string id)
    {
        if (chest != null) UIService.Get.PoolContainer.Return(chest.gameObject);
        
        chest = UIService.Get.PoolContainer.Create<Transform>((GameObject) ContentService.Current.GetObjectByName(id));
        chest.SetParentAndReset(parent);
    }

    public override void OnViewShowCompleted()
    {
        base.OnViewShowCompleted();
        
        InitButtonBase(btnOpen, OnOpenClick);
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        var windowModel = Model as UIChestMessageWindowModel;

        if (isOpen) windowModel.ChestComponent.Rewards.GetInWindow();
        
        windowModel.ChestComponent = null;
    }
    
    private void OnOpenClick()
    {
        isOpen = true;
        Controller.CloseCurrentWindow();
        NSAudioService.Current.Play(SoundId.merge_chest);
    }

    private List<IUIContainerElementEntity> UpdateEntities(List<string> entities)
    {
        var views = new List<IUIContainerElementEntity>(entities.Count);
        
        for (var i = 0; i < entities.Count; i++)
        {
            var def = entities[i];
            
            var entity = new UISimpleScrollElementEntity
            {
                ContentId = def,
                OnSelectEvent = null,
                OnDeselectEvent = null
            };
            
            views.Add(entity);
        }
        
        return views;
    }
}
