using System.Collections.Generic;
using UnityEngine;

public class LevelsDataManager : IECSComponent, IDataManager, IDataLoader<List<LevelsDef>>
{
	public static int ComponentGuid = ECSManager.GetNextGuid();
	public int Guid => ComponentGuid;

	public void OnRegisterEntity(ECSEntity entity)
	{
		Reload();
	}
	
	public void OnUnRegisterEntity(ECSEntity entity)
	{
	}
	
	public List<LevelsDef> Levels;
	
	public void Reload()
	{
		Levels = null;
		LoadData(new ResourceConfigDataMapper<List<LevelsDef>>("configs/levels.data", NSConfigsSettings.Instance.IsUseEncryption));
	}
	
	public void LoadData(IDataMapper<List<LevelsDef>> dataMapper)
	{
		dataMapper.LoadData((data, error) =>
		{
			if (string.IsNullOrEmpty(error))
			{
				data.Sort((a, b) => a.Index.CompareTo(b.Index));
				Levels = new List<LevelsDef>();

				for (var i = 1; i < data.Count; i++)
				{
					var previous = data[i - 1];
					var next = data[i];
					
					next.OrdersWeights = ItemWeight.ReplaseWeights(previous.OrdersWeights, next.OrdersWeights);
					Levels.Add(next);
				}
			}
			else
			{
				Debug.LogWarningFormat("[{0}]: config not loaded", GetType());
			}
		});
	}

	public int Level => ProfileService.Current.GetStorageItem(Currency.Level.Name).Amount;

	public int Price
	{
		get
		{
			var price = Levels[Level - 1].Price;
			return price.Amount;
		}
	}

	public List<CurrencyPair> Rewards => Levels[Level - 1].Rewards;
	
	public int Chest => PieceType.Parse(Levels[Level - 1].Chest);
}