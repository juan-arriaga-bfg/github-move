using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIChestMessageWindowView : UIGenericPopupWindowView
{
    [SerializeField] private NSText btnOpenLabel;
    
    [IWUIBinding("#Delimiter")] private NSText delimiter;
    
    [IWUIBinding("#Chest")] private Image chest;
    [IWUIBinding("#Content")] private UIContainerViewController content;
    
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
        chest.SetNativeSize();
        
        Fill(windowModel.Icons(), content);
        
        var scrollRect = content.GetScrollRect();
        
        if (scrollRect != null) scrollRect.horizontalNormalizedPosition = 0f;
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        var windowModel = Model as UIChestMessageWindowModel;

        if (isOpen) windowModel.ChestComponent.Rewards.GetInWindow();
        
        windowModel.ChestComponent = null;
    }
    
    public void OnOpenClick()
    {
        isOpen = true;
        Controller.CloseCurrentWindow();
    }
    
    public void Fill(List<string> entities, UIContainerViewController container)
    {
        if (entities == null || entities.Count <= 0)
        {
            container.Clear();
            return;
        }
        
        // update items
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
        
        container.Create(views);
        container.Select(0);
    }
}
