using System.Collections.Generic;
using UnityEngine;

public class ObstaclesDataManager : IDataLoader<List<ObstacleDef>>
{
    private List<ObstacleDef> obstacles;
    public List<ObstacleDef> Obstacles
    {
        get { return obstacles; }
    }
    
    public CurrencyPair SimpleObstaclePrice;
    
    public void LoadData(IDataMapper<List<ObstacleDef>> dataMapper)
    {
        dataMapper.LoadData((data, error)=> 
        {
            if (string.IsNullOrEmpty(error))
            {
                obstacles = data;
                SimpleObstaclePrice = new CurrencyPair {Currency = Currency.Coins.Name, Amount = 100};
            }
            else
            {
                Debug.LogWarning("[ObstaclesDataManager]: obstacles config not loaded");
            }
        });
    }

    public int GetChestBySmpleObstacle(int obstacle)
    {
        var common = 0;
        var rare = 0;
        var epic = 0;
        
        var randomValue = Random.Range(0, 100 + 1);
        
        if (obstacle == PieceType.O1.Id)
        {
            rare = 20;
            epic = 5;
        }

        if (obstacle == PieceType.O2.Id)
        {
            rare = 50;
            epic = 10;
        }

        common = (100 - rare) - epic;

        if (randomValue < epic)
        {
            return PieceType.Chest3.Id;
        }
        
        if (randomValue < rare)
        {
            return PieceType.Chest2.Id;
        }
        
        return PieceType.Chest1.Id;
    }
}