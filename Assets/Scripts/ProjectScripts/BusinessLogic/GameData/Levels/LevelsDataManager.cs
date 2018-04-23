using System.Collections.Generic;
using UnityEngine;

public class LevelsDataManager : IDataLoader<List<LevelsDef>>
{
	public List<LevelsDef> Levels;

	public void LoadData(IDataMapper<List<LevelsDef>> dataMapper)
	{
		dataMapper.LoadData((data, error) =>
		{
			if (string.IsNullOrEmpty(error))
			{
				Levels = data;
			}
			else
			{
				Debug.LogWarning("[LevelsDataManager]: levels config not loaded");
			}
		});
	}

	public int Level
	{
		get { return ProfileService.Current.GetStorageItem(Currency.Level.Name).Amount; }
	}

	public int Price
	{
		get
		{
			var price = Levels[Level - 1].Price;
			return price.Amount;
		}
	}
}