using System;
using UnityEngine;

public class CurrencyPair
{
    public string Currency;
    public int Amount;
    
    public const string SaveSeparator = ":";
    
    public string GetIcon()
    {
        var piece = PieceType.Parse(Currency);

        if (piece != PieceType.None.Id) return Currency;
        
        var def = global::Currency.GetCurrencyDef(Currency);
        
        return def.Icon;
    }
    
    public override string ToString()
    {
        return $"{Currency}{SaveSeparator}{Amount}";
    }

    public CurrencyPair Copy()
    {
        return new CurrencyPair {Currency = this.Currency, Amount = this.Amount};
    }
    
    public string ToStringIcon(bool noAmount = false, int iconSize = -1, int amountSize = -1)
    {
        var iconText = iconSize == -1 ? $"<sprite name={GetIcon()}>" : $"<size={iconSize}><sprite name={GetIcon()}></size>";
        var amountText = noAmount ? "" : $"{(amountSize == -1 ? Amount.ToString() : $"<size={amountSize}>{Amount}</size>")}";

        return $"{iconText}{amountText}";
    }

    public static CurrencyPair Parse(string value)
    {
        var valueArray = value.Split(new[] {SaveSeparator}, StringSplitOptions.RemoveEmptyEntries);

        if (valueArray.Length == 0 || string.IsNullOrEmpty(valueArray[0])) return null;
        
        var pair = new CurrencyPair
        {
            Currency = valueArray[0],
            Amount = valueArray.Length > 0 && int.TryParse(valueArray[1], out var amount) ? amount : 0
        };
        
        return pair;
    }
}