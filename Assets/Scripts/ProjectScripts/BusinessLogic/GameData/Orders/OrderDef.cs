using System.Collections.Generic;

public class OrderDef
{
    public string Uid;
    public int Level = -1;
    public int Delay;
    public List<CurrencyPair> Prices;
    public List<CurrencyPair> Rewards;
}