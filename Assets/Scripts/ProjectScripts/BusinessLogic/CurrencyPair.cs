﻿using System;

public class CurrencyPair
{
    public string Currency;
    public int Amount;

    public override string ToString()
    {
        return string.Format("{0}: {1}", Currency, Amount);
    }

    public string ToSaveString()
    {
        var def = global::Currency.GetCurrencyDef(Currency);

        return def == null ? null : string.Format("{0},{1}", def.Id, Amount);
    }

    public string ToStringIcon(bool isLeft = true)
    {
        var icon = Currency == global::Currency.Experience.Name ? "Exp" : string.Format("<sprite name={0}>", Currency);

        return isLeft ? string.Format("{0} {1}", icon, Amount) : string.Format("{0} {1}", Amount, icon);
    }

    public static CurrencyPair Parse(string value)
    {
        var pair = new CurrencyPair();
        var valueArray = value.Split(new string[] {","}, StringSplitOptions.RemoveEmptyEntries);
        var def = global::Currency.GetCurrencyDef(int.Parse(valueArray[0]));

        if (def == null) return null;
        
        pair.Currency = def.Name;
        
        if(valueArray.Length < 2) return pair;

        int.TryParse(valueArray[1], out pair.Amount);
        
        return pair;
    }
}