﻿using System;
using System.Collections.Generic;



public class ChestDef
{
    public string Uid { get; set; }
    public int Time { get; set; }
    
    public CurrencyPair Price { get; set; }
    public Dictionary<int, List<CurrencyPair>> Rewards { get; set; }

    public DateTime? StartTime { get; set; }
}