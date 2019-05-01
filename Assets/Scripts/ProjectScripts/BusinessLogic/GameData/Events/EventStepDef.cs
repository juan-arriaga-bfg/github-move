﻿using System.Collections.Generic;

public class EventStepDef
{
    public string Step;

    public bool IsNormalClaimed;
    public bool IsPremiumClaimed;
    
    public List<CurrencyPair> Prices;
    public List<CurrencyPair> RealPrices;
    
    public List<CurrencyPair> NormalRewards;
    public List<CurrencyPair> PremiumRewards;
}