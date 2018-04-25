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
                        next.ChestWeights = ItemWeight.ReplaseWeights(previous.ChestWeights, next.ChestWeights);
                    }
                    
                    Obstacles.Add(next.Piece, next);
                }
            }
            else
            {
                Debug.LogWarning("[SimpleObstaclesDataManager]: simple obstacles config not loaded");
            }
        });
    }

    public int ChestForPiece(int piece)
    {
        SimpleObstaclesDef def;

        if (Obstacles.TryGetValue(piece, out def) == false) return PieceType.None.Id;
        
        var item = ItemWeight.GetRandomItem(def.ChestWeights);
        
        return item != null ? item.Piece : PieceType.None.Id;
    }

    public Dictionary<int, int> RewardForPiece(int piece, int step)
    {
        SimpleObstaclesDef def;
        
        var result = new Dictionary<int, int>();

        if (Obstacles.TryGetValue(piece, out def) == false) return result;
        
        for (var i = def.PieceAmounts[step - 1] - 1; i >= 0; i--)
        {
            var item = ItemWeight.GetRandomItem(def.PieceWeights[step]);
            
            if(item == null) continue;

            if (result.ContainsKey(item.Piece) == false)
            {
                result.Add(item.Piece, 1);
                continue;
            }
            
            result[item.Piece]++;
        }
        
        return result;
    }

    public CurrencyPair PriceForPiece(Piece piece, int step)
    {
        const int stepPrice = 50;
//        var max = piece.Context.BoardLogic.MatchDefinition.GetIndexInChain(piece.PieceType);

        return new CurrencyPair {Currency = Currency.Coins.Name, Amount = stepPrice + (stepPrice / 2) * step};
    }
}