using System.Collections.Generic;
using UnityEngine;

public class SimpleObstaclesDataManager : IDataLoader<List<SimpleObstaclesDef>>
{
    public Dictionary<int, SimpleObstaclesDef> Obstacles;
    
    public void LoadData(IDataMapper<List<SimpleObstaclesDef>> dataMapper)
    {
        dataMapper.LoadData((data, error)=> 
        {
            Obstacles = new Dictionary<int, SimpleObstaclesDef>();
            
            if (string.IsNullOrEmpty(error))
            {
                data.Sort((a, b) => a.Piece.CompareTo(b.Piece));
                
                Obstacles.Add(data[0].Piece, data[0]);
                
                for (var i = 1; i < data.Count; i++)
                {
                    var def = data[i - 1];
                    var defNext = data[i];
                    var weights = new List<PieceWeight>();

                    foreach (var weight in def.Weights)
                    {
                        var next = defNext.Weights.Find(w => w.Piece == weight.Piece);

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
                    
                    // add values 
                    foreach (var weight in defNext.Weights)
                    {
                        var old = def.Weights.Find(w => w.Piece == weight.Piece);
                        
                        if(old != null) continue;
                        
                        weights.Add(weight.Copy());
                    }

                    defNext.Weights = weights;
                    Obstacles.Add(defNext.Piece, defNext);
                }
            }
            else
            {
                Debug.LogWarning("[HeroesDataManager]: heroes config not loaded");
            }
        });
    }

    public int RewardForPiece(int piece)
    {
        SimpleObstaclesDef def;

        return Obstacles.TryGetValue(piece, out def) == false ? PieceType.None.Id : def.GetReward();
    }

    public CurrencyPair PriceForPiece(Piece piece, int step)
    {
        const int stepPrice = 50;
//        var max = piece.Context.BoardLogic.MatchDefinition.GetIndexInChain(piece.PieceType);

        return new CurrencyPair {Currency = Currency.Coins.Name, Amount = (step + 1) * stepPrice};
    }
}