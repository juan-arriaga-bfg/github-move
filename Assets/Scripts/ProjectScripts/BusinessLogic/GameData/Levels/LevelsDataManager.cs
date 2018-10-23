using System.Collections.Generic;
using UnityEngine;

public class LevelsDataManager : SequenceData, IDataLoader<List<LevelsDef>>
{
	public static int ComponentGuid = ECSManager.GetNextGuid();
	public override int Guid => ComponentGuid;

	public override void OnRegisterEntity(ECSEntity entity)
	{
		Reload();
	}
	
	public List<LevelsDef> Levels;
	
	public override void Reload()
	{
		base.Reload();
		Levels = null;
		LoadData(new ResourceConfigDataMapper<List<LevelsDef>>("configs/levels.data", NSConfigsSettings.Instance.IsUseEncryption));
	}
	
	public void UpdateSequence()
	{
		GetSequence(Currency.Level.Name).Reinit(Levels[Level - 1].PieceWeights);
		GetSequence(Currency.Order.Name).Reinit(Levels[Level - 1].OrdersWeights);
	}

	public void LoadData(IDataMapper<List<LevelsDef>> dataMapper)
	{
		dataMapper.LoadData((data, error) =>
		{
			if (string.IsNullOrEmpty(error))
			{
				data.Sort((a, b) => a.Index.CompareTo(b.Index));
				Levels = new List<LevelsDef>{data[0]};
				
				for (var i = 1; i < data.Count; i++)
				{
					var previous = data[i - 1];
					var next = data[i];
					
					next.OrdersWeights = ItemWeight.ReplaseWeights(previous.OrdersWeights, next.OrdersWeights);
					next.PieceWeights = ItemWeight.ReplaseWeights(previous.PieceWeights, next.PieceWeights);
					Levels.Add(next);
				}
				
				AddSequence(Currency.Level.Name, Levels[Level - 1].PieceWeights);
				AddSequence(Currency.Order.Name, Levels[Level - 1].OrdersWeights);
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
	public List<ItemWeight> PieceWeights => Levels[Level - 1].PieceWeights;
}