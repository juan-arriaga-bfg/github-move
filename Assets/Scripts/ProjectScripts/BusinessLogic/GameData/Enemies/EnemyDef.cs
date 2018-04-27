using System.Collections.Generic;

public class EnemyDef
{
    public string Uid { get; set; }
    public int Level { get; set; }
    
    public List<CurrencyPair> Steps { get; set; }
}