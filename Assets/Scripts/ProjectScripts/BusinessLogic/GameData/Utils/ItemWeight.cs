using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemWeight
{
    private int pieceType = -1;
    
    public string Uid { get; set; }
    public int Weight { get; set; }
    
    public int Piece => pieceType == -1 ? (pieceType = PieceType.Parse(Uid)) : pieceType;

    public ItemWeight Copy()
    {
        return new ItemWeight{Uid = this.Uid, Weight = this.Weight};
    }

    public override string ToString()
    {
        return $"Uid: {Uid} - Weight: {Weight}";
    }
    
    public static ItemWeight GetRandomItem(List<ItemWeight> weights)
    {
        var sum = weights.Sum(w => w.Weight);
        var current = 0;
        var random = Random.Range(0, sum + 1);
        
        weights.Sort((a, b) => a.Weight.CompareTo(b.Weight));
        
        foreach (var item in weights)
        {
            current += item.Weight;
            
            if (current < random) continue;
            
            return item;
        }
        
        return null;
    }
    
    public static int GetRandomItemIndex(List<ItemWeight> weights)
    {
        var item = GetRandomItem(weights);
        return item == null ? -1 : weights.IndexOf(item);
    }

    public static List<byte> GetRandomSequence(List<ItemWeight> weights, int seed = -1)
    {
        var result = new List<byte>();

        for (byte i = 0; i < weights.Count; i++)
        {
            var item = weights[i];
            
            for (var j = 0; j < item.Weight; j++)
            {
                result.Add(i);
            }
        }
        
        if (result.Count == 0) return result;
        
        result.Shuffle(seed);

        return result;
    }
    
    public static List<ItemWeight> ReplaceWeights(List<ItemWeight> oldWeights, List<ItemWeight> nextWeights)
    {
        var weights = new List<ItemWeight>();
        
        if (oldWeights == null) oldWeights = new List<ItemWeight>();
        if (nextWeights == null) nextWeights = new List<ItemWeight>();
        
        foreach (var item in oldWeights)
        {
            var next = nextWeights.Find(w => w.Uid == item.Uid);

            if (next != null) continue;
            
            weights.Add(item.Copy());
        }

        foreach (var item in nextWeights)
        {
            if (item.Weight == 0) continue;
            
            weights.Add(item.Copy());
        }
        
        return weights;
    }
}