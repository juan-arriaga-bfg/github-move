using System.Collections.Generic;

public class UIHardShopWindowView : UIGenericPopupWindowView 
{
    [IWUIBinding("#Content")] private UIContainerViewController content;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UIHardShopWindowModel;
        
        SetTitle(windowModel.Title);
        SetMessage(windowModel.Message);
        
        Fill(UpdateEntities(windowModel.Products), content);
        
        var tabsScrollRect = content.GetScrollRect();
        if (tabsScrollRect != null)
        {
            tabsScrollRect.verticalNormalizedPosition = 1f;
        }
    }
    
    private List<IUIContainerElementEntity> UpdateEntities(List<ShopDef> entities)
    {
        var views = new List<IUIContainerElementEntity>(entities.Count);
        
        for (var i = 0; i < entities.Count; i++)
        {
            var def = entities[i];
            
            var entity = new UIShopElementEntity
            {
                ContentId = def.Icon,
                PurchaseKey = def.PurchaseKey,
                NameLabel = LocalizationService.Get(def.Name, def.Name),
                Products = def.Products,
                Price = def.Price,
                OnSelectEvent = null,
                OnDeselectEvent = null
            };
            
            views.Add(entity);
        }
        
        return views;
    }
}
