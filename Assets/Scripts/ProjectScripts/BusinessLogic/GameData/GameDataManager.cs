using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PieceWeight
{
    private int pieceType = -1;
    
    public string Uid { get; set; }
    public int Weight { get; set; }
    public bool Override { get; set; }
    
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

    public PieceWeight Copy()
    {
        return new PieceWeight{Uid = this.Uid, Weight = this.Weight};
    }

    public override string ToString()
    {
        return string.Format("Uid: {0} - Weight: {1} - Override: {2}", Uid, Weight, Override);
    }

    public static int GetRandomPiece(List<PieceWeight> weights)
    {
        var sum = weights.Sum(w => w.Weight);
        var current = 0;
        var random = Random.Range(0, sum + 1);
        
        weights.Sort((a, b) => a.Weight.CompareTo(b.Weight));
        
        foreach (var item in weights)
        {
            current += item.Weight;
            
            if (current < random) continue;
            
            return item.Piece;
        }
        
        return PieceType.None.Id;
    }

    public static List<PieceWeight> ReplaseWeights(List<PieceWeight> oldWeights, List<PieceWeight> nextWeights)
    {
        var weights = new List<PieceWeight>();

        foreach (var weight in oldWeights)
        {
            var next = nextWeights.Find(w => w.Piece == weight.Piece);

            if (next == null)
            {
                weights.Add(weight.Copy());
                continue;
            }
                        
            if (next.Override)
            {
                weights.Add(next.Copy());
                continue;
            }
                        
            var newWeight = weight.Copy();

            newWeight.Weight += next.Weight;

            weights.Add(newWeight);
        }
                    
        foreach (var weight in nextWeights)
        {
            var old = oldWeights.Find(w => w.Piece == weight.Piece);
                        
            if(old != null) continue;
                        
            weights.Add(weight.Copy());
        }

        return weights;
    }
}


public class GameDataManager
{
    public readonly ChestsDataManager ChestsManager = new ChestsDataManager();
    public readonly EnemiesDataManager EnemiesManager = new EnemiesDataManager();
    public readonly HeroesDataManager HeroesManager = new HeroesDataManager();
    public readonly PiecesDataManager PiecesManager = new PiecesDataManager();
    public readonly ObstaclesDataManager ObstaclesManager = new ObstaclesDataManager();
    public readonly SimpleObstaclesDataManager SimpleObstaclesManager = new SimpleObstaclesDataManager();
    public readonly QuestsDataManager QuestsManager = new QuestsDataManager();
    public readonly FogsDataManager FogsManager = new FogsDataManager();
}