﻿using System;

public class CurrencyPair
{
    public string Currency;
    public int Amount;

    public override string ToString()
    {
        return $"{Currency}: {Amount}";
    }

    public string ToSaveString()
    {
        var def = global::Currency.GetCurrencyDef(Currency);

        return def == null ? null : $"{def.Id},{Amount}";
    }

    public string ToStringIcon(bool isLeft = true)
    {
        var icon = $"<sprite name={Currency}>";
        return isLeft ? $"{icon} {Amount}" : $"{Amount} {icon}";
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