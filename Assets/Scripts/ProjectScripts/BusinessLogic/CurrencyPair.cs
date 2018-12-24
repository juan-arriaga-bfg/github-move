using System;
using UnityEngine;

public class CurrencyPair
{
    public string Currency;
    public int Amount;
    
    public Sprite GetIconSprite()
    {
        return IconService.Current.GetSpriteById(GetIcon());
    }
    
    public string GetIcon()
    {
        var piece = PieceType.Parse(Currency);

        if (piece != PieceType.None.Id) return Currency;
        
        var def = global::Currency.GetCurrencyDef(Currency);
        
        return def.Icon;
    }
    
    public override string ToString()
    {
        return $"{Currency}: {Amount}";
    }

    public string ToSaveString()
    {
        var def = global::Currency.GetCurrencyDef(Currency);

        return def == null ? null : $"{def.Id},{Amount}";
    }

    public string ToStringIcon(bool noAmount = false, int iconSize = -1, int amountSize = -1)
    {
        var iconText = iconSize == -1 ? $"<sprite name={GetIcon()}>" : $"<size={iconSize}><sprite name={GetIcon()}></size>";
        var amountText = noAmount ? "" : $"{(amountSize == -1 ? Amount.ToString() : $"<size={amountSize}>{Amount}</size>")}";

        return $"{iconText}{amountText}";
    }

    public static CurrencyPair Parse(string value)
    {
        var pair = new CurrencyPair();
        var valueArray = value.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
        var def = global::Currency.GetCurrencyDef(int.Parse(valueArray[0]));

        if (def == null) return null;
        
        pair.Currency = def.Name;
        
        if(valueArray.Length < 2) return pair;

        int.TryParse(valueArray[1], out pair.Amount);
        
        return pair;
    }
}