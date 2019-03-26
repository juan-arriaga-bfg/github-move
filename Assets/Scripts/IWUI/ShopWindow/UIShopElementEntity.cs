﻿using System.Collections.Generic;
using System.Text;

public class UIShopElementEntity : UISimpleScrollElementEntity
{
    public override string LabelText
    {
        get
        {
            if (Products.Count > 1) return LocalizationService.Get("common.message.sale", "common.message.sale");
            
            var str = new StringBuilder();
            
            foreach (var pair in Products)
            {
                str.Append($"{pair.ToStringIcon(false, MessageIconSize)}\n");
            }
            
            return str.ToString().TrimEnd();
        }
    }
    public string ExtraText
    {
        get
        {
            if (Extras == null) return string.Empty;
            
            var str = new StringBuilder();
            
            foreach (var pair in Extras)
            {
                str.Append($"+{pair.ToStringIcon(false, MessageIconSize)}\n");
            }
            
            return str.ToString().TrimEnd();
        }
    }

    public string ButtonLabel
    {
        get
        {
            string ret;
            
            if (Price.Currency == Currency.Cash.Name)
            {
                ret = IapService.Current.GetLocalizedPriceStr(PurchaseKey);
                
                if (string.IsNullOrEmpty(ret)) ret = $"${Price.Amount}";
            }
            else
            {
                ret = string.Format(LocalizationService.Get("common.button.buy", "common.button.buy {0}"), Price.ToStringIcon());
            }
            
            return ret;
        }
    }

    public int MessageIconSize = -1;
    
    public string PurchaseKey => Def.PurchaseKey;

    public string NameLabel;

    public bool IsPermanent => Def.IsPermanent;
    
    public List<CurrencyPair> Products => Def.Products;
    public List<CurrencyPair> Extras => Def.Extras;
    public CurrencyPair Price => Def.Price;

    public ShopDef Def;
}