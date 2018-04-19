using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SimpleObstaclesDef
{ 
    private int pieceType = -1;
    private int weights = -1;
    
    public string Uid { get; set; }

    public List<PieceWeight> Weights;
    
    public int Piece
    {
        get
        {
            if (pieceType == -1)
            {
                pieceType = PieceType.Parse(Uid);
            }
            
            return pieceType;
        }
    }

    public int GetReward()
    {
        if (weights == -1)
        {
            Weights.Sort((a, b) => a.Weight.CompareTo(b.Weight));
            weights = Weights.Sum(item => item.Weight) + 1;
        }
        
        var current = 0;
        var random = Random.Range(0, weights);
        
        foreach (var item in Weights)
        {
            current += item.Weight;
            
            if (current < random) continue;
            
            return item.Piece;
        }

        return PieceType.None.Id;
    }

    public override string ToString()
    {
        var str = string.Format("Uid: {0}, Piece: {1}", Uid, Piece);

        foreach (var weight in Weights)
        {
            str += string.Format("\n{0}", weight);
        }

        return str;
    }
}