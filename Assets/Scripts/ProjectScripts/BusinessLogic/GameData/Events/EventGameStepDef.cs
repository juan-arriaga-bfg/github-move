using System.Collections.Generic;

public class EventGameStepDef
{
    public string Step;

    public bool IsNormalClaimed;
    public bool IsPremiumClaimed;
    
    public bool IsNormalIgnored => NormalRewards == null || NormalRewards.Count == 0;
    public bool IsPremiumIgnored => PremiumRewards == null || PremiumRewards.Count == 0;
    
    public bool IsNormalIgnoredOrClaimed => IsNormalIgnored || IsNormalClaimed;
    public bool IsPremiumIgnoredOrClaimed => IsPremiumIgnored || IsPremiumClaimed;
    
    public List<CurrencyPair> Prices;
    public List<CurrencyPair> RealPrices;
    
    public List<CurrencyPair> NormalRewards;
    public CurrencyPair NormalRewardsPrice;
    
    public List<CurrencyPair> PremiumRewards;
    public CurrencyPair PremiumRewardsPrice;
}