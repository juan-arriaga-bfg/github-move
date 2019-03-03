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
            
            var entity = new UIShopElementEntity
            {
                ContentId = def.Icon,
                PurchaseKey = def.PurchaseKey,
                Products = def.Products,
                Price = def.Price,
                NameLabel = LocalizationService.Get(def.Name),
                IsPermanent = def.IsPermanent,
                OnSelectEvent = null,
                OnDeselectEvent = null
            };
            
            views.Add(entity);
        }
        
        return views;
    }
}
