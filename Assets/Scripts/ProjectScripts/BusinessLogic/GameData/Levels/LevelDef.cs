using System.Collections.Generic;

public class LevelsDef
{
    public int Index;
    public string Chest;
    public CurrencyPair Price;
    public List<CurrencyPair> Rewards;
    public int PieceAmount;
    
    public List<ItemWeight> PieceWeights = new List<ItemWeight>();
    public List<ItemWeight> OrdersWeights = new List<ItemWeight>();
}