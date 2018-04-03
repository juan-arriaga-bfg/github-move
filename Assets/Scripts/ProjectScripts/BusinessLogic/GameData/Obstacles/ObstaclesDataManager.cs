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
}