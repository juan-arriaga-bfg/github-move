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

    public override void OnViewCloseCompleted()
    {
        UIShopElementEntity entity = null;

        foreach (UIShopElementViewController tab in content.Tabs)
        {
            if(tab.IsNeedReopen == false) continue;

            entity = tab.Entity as UIShopElementEntity;
            break;
        }
        
        base.OnViewCloseCompleted();
        
        if(entity == null) return;
        
        CurrencyHelper.OpenShopWindow(entity.Price.Currency);
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
