using System.Collections.Generic;

public class EventGameStepDef
{
    public string Step;

    public bool IsNormalClaimed;
    public bool IsPremiumClaimed;
    
    public bool IsNormalIgnoredOrClaimed => NormalRewards == null || NormalRewards.Count == 0 || IsNormalClaimed;
    public bool IsPremiumIgnoredOrClaimed => PremiumRewards == null || PremiumRewards.Count == 0 || IsPremiumClaimed;
    
    public List<CurrencyPair> Prices;
    public List<CurrencyPair> RealPrices;
    
    public List<CurrencyPair> NormalRewards;
    public List<CurrencyPair> PremiumRewards;
}