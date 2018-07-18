using UnityEngine;

public class UIEnergyShopItem : MonoBehaviour
{
    [SerializeField] private NSText product;
    [SerializeField] private NSText price;
    
    private ShopDef def;
    
    public void Init(ShopDef def)
    {
        this.def = def;

        product.Text = def.Product.ToStringIcon();
        price.Text = string.Format("Buy for {0}", def.Price.ToStringIcon());
    }

    public void OnClick()
    {
        CurrencyHellper.Purchase(def.Product, def.Price, success =>
        {
            if (success == false) return;

            UIService.Get.CloseWindow(UIWindowType.EnergyShopWindow, true);
        });
    }
}