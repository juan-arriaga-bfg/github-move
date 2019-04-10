using System.Collections.Generic;

public class LevelsDef
{
    public int Index;
    
    public CurrencyPair Price;
    public List<CurrencyPair> Rewards;
    
    public List<ItemWeight> PieceWeights = new List<ItemWeight>();
    public List<ItemWeight> OrdersWeights = new List<ItemWeight>();
    public List<ItemWeight> ResourcesWeights = new List<ItemWeight>();
    public List<ItemWeight> ExtrasWeights = new List<ItemWeight>();
    
    public List<CurrencyPair> HardOrders;

    private List<string> uids;
    public List<string> HardOrdersUids
    {
        get
        {
            if (HardOrders == null) return null;
            if (uids != null) return uids;
            
            uids = new List<string>();

            foreach (var pair in HardOrders)
            {
                if(uids.Contains(pair.Currency)) continue;
                uids.Add(pair.Currency);
            }
            
            return uids;
        }
    }
    
    public int OrdersDelay;
}