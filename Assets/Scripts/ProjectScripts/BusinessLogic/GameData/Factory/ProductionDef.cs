using System.Collections.Generic;

public class ProductionDef
{
    public string Uid { get; set; }
    public int Delay { get; set; }
    public string Target { get; set; }
    public List<CurrencyPair> Prices { get; set; }
}