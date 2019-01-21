﻿using System.Collections.Generic;
using System.Text;

public class UIShopElementEntity : UISimpleScrollElementEntity
{
    public override string LabelText
    {
        get
        {
            var str = new StringBuilder();
            
            foreach (var pair in Products)
            {
                str.Append($"{pair.ToStringIcon(false, 35)}\n");
            }
            
            return str.ToString().TrimEnd();
        }
    }

    public string ButtonLabel => Price.Currency == Currency.Cash.Name ? $"${Price.Amount}" : string.Format(LocalizationService.Get("common.button.buy", "common.button.buy {0}"), Price.ToStringIcon());
    
    public string PurchaseKey;

    public string NameLabel;
    
    public List<CurrencyPair> Products;
    public CurrencyPair Price;
}