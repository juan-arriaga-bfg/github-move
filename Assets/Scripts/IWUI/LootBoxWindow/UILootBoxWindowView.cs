using UnityEngine;
using System.Collections.Generic;

public class UILootBoxWindowView : UIGenericPopupWindowView 
{
    [IWUIBinding("#Content")] private UIContainerViewController content;
    
    [IWUIBinding("#IconAnchor")] private Transform anchor;
    
    [IWUIBinding("#NameLabel")] private NSText itemName;
    [IWUIBinding("#Amount")] private NSText itemAmount;
    [IWUIBinding("#ButtonAcceptLabel")] private NSText buttonAcceptLabel;
    
    [IWUIBinding("#ButtonAccept")] private UIButtonViewController btnAccept;
    
    [IWUIBinding("#IslandItem")] private GameObject itemIsland;
    [IWUIBinding("#ChestItem")] private GameObject itemChest;
    
    private Transform icon;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UILootBoxWindowModel;
        
        SetTitle(windowModel.Title);
        SetMessage(windowModel.Message);
        
        buttonAcceptLabel.Text = windowModel.Button;
        itemName.Text = windowModel.ItemName;
        itemAmount.Text = windowModel.ItemAmount;
        
        CreateIcon(anchor, windowModel.ItemIcon);
        Fill(UpdateEntities(windowModel.Probability), content);
        
        itemIsland.SetActive(windowModel.IsIsland);
        itemChest.SetActive(!windowModel.IsIsland);
    }

    public override void OnViewShowCompleted()
    {
        base.OnViewShowCompleted();
        
        InitButtonBase(btnAccept, Controller.CloseCurrentWindow);
    }

    public override void OnViewCloseCompleted()
    {
        if (icon != null) UIService.Get.PoolContainer.Return(icon.gameObject);
        
        base.OnViewCloseCompleted();
    }

    private void CreateIcon(Transform parent, string id)
    {
        icon = UIService.Get.PoolContainer.Create<Transform>((GameObject) ContentService.Current.GetObjectByName(id));
        icon.SetParentAndReset(parent);
    }

    protected virtual List<IUIContainerElementEntity> UpdateEntities(List<KeyValuePair<string, string>> entities)
    {
        var views = new List<IUIContainerElementEntity>(entities.Count);
        
        for (var i = 0; i < entities.Count; i++)
        {
            var def = entities[i];
            
            var entity = new UISimpleScrollElementEntity
            {
                ContentId = def.Key,
                LabelText = def.Value,
                OnSelectEvent = null,
                OnDeselectEvent = null
            };
            
            views.Add(entity);
        }
        
        return views;
    }
}
