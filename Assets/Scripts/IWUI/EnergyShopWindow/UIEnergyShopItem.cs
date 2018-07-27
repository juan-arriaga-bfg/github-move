﻿using UnityEngine;
using UnityEngine.UI;

public class UIEnergyShopItem : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private NSText product;
    [SerializeField] private NSText price;
    
    private ChestDef chest;
    
    public void Init(ChestDef def)
    {
        chest = def;
        
        icon.sprite = IconService.Current.GetSpriteById(chest.Uid);
        icon.SetNativeSize();
        
        var resource = CurrencyHellper.ResourcePieceToCurrence(def.GetHardPieces(), Currency.Energy.Name);
        
        product.Text = string.Format("{0}: +{1}", Currency.Energy, resource.ToStringIcon(false));
        price.Text = string.Format("Buy {0}", def.Price.ToStringIcon(false));
    }

    public void OnClick()
    {
        CurrencyHellper.Purchase(Currency.Chest.Name, 1, chest.Price, success =>
        {
            if(success == false) return;
			
            var model = UIService.Get.GetCachedModel<UIEnergyShopWindowModel>(UIWindowType.EnergyShopWindow);
            model.ChestReward = chest.Piece;
            UIService.Get.CloseWindow(UIWindowType.EnergyShopWindow, true);
        });
    }
}