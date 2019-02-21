using UnityEngine;

public class UIHardShopElementViewController : UIShopElementViewController
{
    [IWUIBinding("#BackPurple")] protected GameObject backPurple;
    [IWUIBinding("#BackBrown")] protected GameObject backBrown;
    
    public override void Init()
    {
        base.Init();
        
        var contentEntity = entity as UIShopElementEntity;
        var isPack = contentEntity.Products.Count > 1;
        
        label.StyleId = isPack ? 15 : 14;
        label.ApplyStyle();
        
        backPurple.SetActive(isPack);
        backBrown.SetActive(!isPack);
    }

    protected override string GetAnalyticLocation()
    {
        return "shop_premium";
    }
}