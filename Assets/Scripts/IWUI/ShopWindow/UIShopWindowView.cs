using System.Collections.Generic;

public class UIShopWindowView : UIGenericPopupWindowView 
{
    [IWUIBinding("#Content")] protected UIContainerViewController content;
    
    [IWUIBinding("#CloseMaskLeft")] protected UIButtonViewController btnMaskLeft;
    [IWUIBinding("#CloseMaskRight")] protected UIButtonViewController btnMaskRight;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UIShopWindowModel;
        
        SetTitle(windowModel.Title);
        SetMessage(windowModel.Message);
        
        Fill(UpdateEntities(windowModel.Products), content);
        content.GetScrollRect().horizontalNormalizedPosition = 0f;
    }

    public override void OnViewShowCompleted()
    {
        base.OnViewShowCompleted();
        
        InitButtonBase(btnMaskLeft, Controller.CloseCurrentWindow);
        InitButtonBase(btnMaskRight, Controller.CloseCurrentWindow);
    }
    
    protected virtual List<IUIContainerElementEntity> UpdateEntities(List<ShopDef> entities)
    {
        var views = new List<IUIContainerElementEntity>(entities.Count);
        
        for (var i = 0; i < entities.Count; i++)
        {
            var def = entities[i];
            var isPack = def.Sale > 0;
            
            var entity = new UIShopElementEntity
            {
                ContentId = isPack ? string.Empty : def.Icon,
                NameLabel = isPack ? string.Empty : LocalizationService.Get(def.Name),
                Def = def,
                OnSelectEvent = null,
                OnDeselectEvent = null
            };
            
            views.Add(entity);
        }
        
        return views;
    }
}
