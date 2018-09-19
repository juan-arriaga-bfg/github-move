using System.Collections.Generic;

public class LevelsDef
{
    public int Index { get; set; }
    public string Chest { get; set; }
    public CurrencyPair Price { get; set; }
    public List<CurrencyPair> Rewards { get; set; }
}