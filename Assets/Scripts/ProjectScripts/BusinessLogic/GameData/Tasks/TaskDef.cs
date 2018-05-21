using System.Collections.Generic;

public class TaskDef
{
    public int Uid { get; set; }
    public string Message { get; set; }
    public string Character { get; set; }
    public CurrencyPair Result { get; set; }
    public List<CurrencyPair> Prices { get; set; }
    public List<CurrencyPair> Rewards { get; set; }
}