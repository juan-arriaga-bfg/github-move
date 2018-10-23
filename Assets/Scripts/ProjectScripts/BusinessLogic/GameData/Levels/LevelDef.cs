using System.Collections.Generic;

public class LevelsDef
{
    public int Index;
    public string Chest;
    public CurrencyPair Price;
    public List<CurrencyPair> Rewards;
    public List<ItemWeight> OrdersWeights = new List<ItemWeight>();
}