﻿using System.Collections.Generic;

public class ChestDef
{
    public string Uid { get; set; }
    public int Time { get; set; }
    public int MergePoints { get; set; }
    public CurrencyPair Price { get; set; }
    public List<List<CurrencyPair>> Rewards { get; set; }
}