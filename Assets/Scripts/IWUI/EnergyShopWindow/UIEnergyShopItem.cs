using UnityEngine;
using UnityEngine.UI;

public class UIEnergyShopItem : IWUIWindowViewController
{
    [SerializeField] private Transform anchor;
    [SerializeField] private NSText product;
    [SerializeField] private NSText price;
    
    private ShopDef def;
    
    private bool isClick;
    
    public void Init(ShopDef def)
    {
        this.def = def;
        
        isClick = false;

        var tr = UIService.Get.PoolContainer.Create<Transform>((GameObject) ContentService.Current.GetObjectByName(def.Icon));
        tr.SetParentAndReset(anchor);
        
        product.Text = $"+{def.Product.Amount}<size=35> <sprite name={def.Product.Currency}></size>";
        price.Text = $"Buy {def.Price.ToStringIcon(false)}";
    }

    public override void OnViewCloseCompleted()
    {
        if (isClick == false) return;
        
        CurrencyHellper.Purchase(def.Product, def.Price, null, new Vector2(Screen.width/2, Screen.height/2));
    }

    public void OnClick()
    {
        if(isClick) return;
		
        isClick = true;
        
        if (CurrencyHellper.IsCanPurchase(def.Price, true) == false)
        {
            isClick = false;
            return;
        }
        
        context.Controller.CloseCurrentWindow();
    }
}