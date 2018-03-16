using System.Collections.Generic;

public class HeroDef
{
    public string Uid { get; set; }
    public List<int> TimeBonuses { get; set; }
    public List<CurrencyPair> Prices { get; set; }

    public override string ToString()
    {
        var str = string.Format("Uid: {0}\nDamages:\n", Uid);

        for (int i = 0; i < TimeBonuses.Count; i++)
        {
            str += string.Format(" level {0} - {1},", i+1, TimeBonuses[i]);
        }

        str += "\nPrices:\n";
        
        for (int i = 0; i < Prices.Count; i++)
        {
            str += string.Format(" level {0} - {1}:{2},", i+1, Prices[i].Currency, Prices[i].Amount);
        }
        
        return str;
    }
}