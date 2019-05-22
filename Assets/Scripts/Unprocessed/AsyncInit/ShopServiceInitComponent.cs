using System.Collections.Generic;

public class ShopServiceInitComponent : AsyncInitComponentBase
{
    public override void Execute()
    {
        //init shopmanager
        ShopManager shopManager = new ShopManager();
        shopManager.InitStorage(
            new ResourceConfigDataMapper<IEnumerable<ShopItem>>("configs/shopitems.data",
                NSConfigsSettings.Instance.IsUseEncryption),
            (shopItems) => { });
         
        ShopService.Instance.SetManager(shopManager);

        isCompleted = true;
        OnComplete(this);
    }
}