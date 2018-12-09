using System.Collections.Generic;

public class UISoftShopWindowView : UIGenericPopupWindowView 
{
    [IWUIBinding("#Content")] private UIContainerViewController content;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UISoftShopWindowModel;
        
        SetTitle(windowModel.Title);
        SetMessage(windowModel.Message);
        
        Fill(UpdateEntities(windowModel.Products), content);
    }
    
    private List<IUIContainerElementEntity> UpdateEntities(List<ShopDef> entities)
    {
        var views = new List<IUIContainerElementEntity>(entities.Count);
        
        for (var i = 0; i < entities.Count; i++)
        {
            var def = entities[i];
            
            var entity = new UISoftShopElementEntity
            {
                ContentId = def.Icon,
                Product = def.Product,
                Price = def.Price,
                OnSelectEvent = null,
                OnDeselectEvent = null
            };
            
            views.Add(entity);
        }
        
        return views;
    }
}
