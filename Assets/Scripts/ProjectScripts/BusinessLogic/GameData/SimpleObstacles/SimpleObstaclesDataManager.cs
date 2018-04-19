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
            var matchDefinition = new MatchDefinitionComponent(new MatchDefinitionBuilder().Build());
            
            if (string.IsNullOrEmpty(error))
            {
                data.Sort((a, b) => a.Piece.CompareTo(b.Piece));

                foreach (var next in data)
                {
                    var previousType = matchDefinition.GetPrevious(next.Piece);

                    if (previousType != PieceType.None.Id)
                    {
                        var previous = data.Find(def => def.Piece == previousType);
                        next.Weights = PieceWeight.ReplaseWeights(previous.Weights, next.Weights);
                    }
                    
                    Obstacles.Add(next.Piece, next);
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

        return Obstacles.TryGetValue(piece, out def) == false ? PieceType.None.Id : PieceWeight.GetRandomPiece(def.Weights);
    }

    public CurrencyPair PriceForPiece(Piece piece, int step)
    {
        const int stepPrice = 50;
//        var max = piece.Context.BoardLogic.MatchDefinition.GetIndexInChain(piece.PieceType);

        return new CurrencyPair {Currency = Currency.Coins.Name, Amount = (step + 1) * stepPrice};
    }
}