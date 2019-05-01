using Debug = IW.Logger;
using System.Collections.Generic;

public class LevelsDataManager : SequenceData, IDataLoader<List<LevelsDef>>
{
	public static int ComponentGuid = ECSManager.GetNextGuid();
	public override int Guid => ComponentGuid;
	
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
		GetSequence(Currency.Order.Name).Reinit(Levels[Level - 1].OrdersWeights, Levels[Level - 1].HardOrdersUids);
		GetSequence(Currency.Resources.Name).Reinit(Levels[Level - 1].ResourcesWeights);
		GetSequence(Currency.Extra.Name).Reinit(Levels[Level - 1].ExtrasWeights);
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
					
					next.OrdersWeights = ItemWeight.ReplaceWeights(previous.OrdersWeights, next.OrdersWeights);
					next.PieceWeights = ItemWeight.ReplaceWeights(previous.PieceWeights, next.PieceWeights);
					next.ResourcesWeights = ItemWeight.ReplaceWeights(previous.ResourcesWeights, next.ResourcesWeights);
					next.ExtrasWeights = ItemWeight.ReplaceWeights(previous.ExtrasWeights, next.ExtrasWeights);
					
					Levels.Add(next);
				}
				
				AddSequence(Currency.Level.Name, Levels[Level - 1].PieceWeights);
				AddSequence(Currency.Resources.Name, Levels[Level - 1].ResourcesWeights);
				AddSequence(Currency.Order.Name, Levels[Level - 1].OrdersWeights, Levels[Level - 1].HardOrdersUids);
				AddSequence(Currency.Extra.Name, Levels[Level - 1].ExtrasWeights);
			}
			else
			{
				Debug.LogWarningFormat("[{0}]: config not loaded", GetType());
			}
		});
	}

	public int Level => ((GameDataManager)context).UserProfile.GetStorageItem(Currency.Level.Name).Amount;

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
	public List<ItemWeight> ResourcesWeights => Levels[Level - 1].ResourcesWeights;
	public List<ItemWeight> ExtrasWeights => Levels[Level - 1].ExtrasWeights;
	
    public int OrdersDelay => Levels[Level - 1].OrdersDelay;
}