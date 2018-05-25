using System.Collections.Generic;
using System.Linq;

public class Task
{
    private TaskDef def;

    public TaskDef Def
    {
        get { return def; }
    }
    
    public Task(TaskDef def)
    {
        this.def = def;
    }

    public CurrencyPair GetHardPrice()
    {
        List<CurrencyPair> diff;

        if (CurrencyHellper.IsCanPurchase(def.Prices, out diff)) return null;
        
        var amount = diff.Sum(pair => pair.Amount);
        
        return new CurrencyPair{Currency = Currency.Crystals.Name, Amount = amount};
    }

    public bool IsComplete
    {
        get { return CurrencyHellper.IsCanPurchase(def.Prices); }
    }
    
    public bool Exchange()
    {
        var isSuccess = false;
        List<CurrencyPair> diff;
        
        if (CurrencyHellper.IsCanPurchase(def.Prices, out diff))
        {
            CurrencyHellper.Purchase(def.Result, def.Prices, success =>
            {
                isSuccess = success;
                if(success == false) return;

                foreach (var reward in def.Rewards)
                {
                    CurrencyHellper.Purchase(reward);
                }
            });
            
            return isSuccess;
        }

        var amount = diff.Sum(pair => pair.Amount);
        
        CurrencyHellper.Purchase(def.Result, new CurrencyPair{Currency = Currency.Crystals.Name, Amount = amount}, success =>
        {
            isSuccess = success;
            if(success == false) return;

            foreach (var pair in diff)
            {
                CurrencyHellper.Purchase(pair);
            }
            
            CurrencyHellper.Purchase(def.Rewards[0], def.Prices);

            for (var i = 1; i < def.Rewards.Count; i++)
            {
                var reward = def.Rewards[i];
                CurrencyHellper.Purchase(reward);
            }
        });
        
        return isSuccess;
    }
}