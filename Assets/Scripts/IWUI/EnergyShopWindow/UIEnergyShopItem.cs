using UnityEngine;
using UnityEngine.UI;

public class UIEnergyShopItem : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private NSText product;
    [SerializeField] private NSText price;
    
    private ShopDef def;
    
    public void Init(ShopDef def)
    {
        this.def = def;

        icon.sprite = IconService.Current.GetSpriteById(this.def.Uid);
        icon.SetNativeSize();
        
        product.Text = string.Format("{0}: +{1}", def.Product.Currency, def.Product.ToStringIcon(false));
        price.Text = string.Format("Buy {0}", def.Price.ToStringIcon(false));
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