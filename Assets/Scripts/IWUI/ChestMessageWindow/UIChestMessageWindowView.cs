using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIChestMessageWindowView : UIGenericPopupWindowView
{
    [IWUIBinding("#Delimiter")] private NSText delimiter;
    [IWUIBinding("#OpenButtonLabel")] private NSText btnOpenLabel;
    
    [IWUIBinding("#Chest")] private Image chest;
    [IWUIBinding("#Content")] private UIContainerViewController content;
    [IWUIBinding("#OpenButton")] private UIButtonViewController btnOpen;
    
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
        
        chest.sprite = IconService.Current.GetSpriteById(windowModel.ChestComponent.Def.Uid);
        
        Fill(UpdateEntities(windowModel.Icons()), content);
        
        var scrollRect = content.GetScrollRect();
        
        if (scrollRect != null) scrollRect.horizontalNormalizedPosition = 0f;
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
