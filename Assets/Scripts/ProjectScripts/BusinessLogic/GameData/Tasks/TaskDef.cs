using System.Collections.Generic;
using UnityEngine;

public class ItemRange
{
    public int Min { get; set; }
    public int Max { get; set; }

    public int GetValue()
    {
        return Random.Range(Min, Max + 1);
    }
}

public class TaskDef
{
    public string Resource { get; set; }
    public int Level { get; set; }
    public ItemRange Range { get; set; }
    public List<CurrencyPair> Rewards { get; set; }
    public int Weight { get; set; }
    
    public ItemWeight GetItemWeight()
    {
        return new ItemWeight {Uid = Resource, Weight = this.Weight};
    }
}