using System.Collections.Generic;

public class LevelsDef
{
    public int Index;
    
    public CurrencyPair Price;
    public List<CurrencyPair> Rewards;
    
    public List<ItemWeight> PieceWeights = new List<ItemWeight>();
    public List<ItemWeight> OrdersWeights = new List<ItemWeight>();
    public List<ItemWeight> ResourcesWeights = new List<ItemWeight>();

    public int OrdersDelay;
}