using System.Collections.Generic;
using UnityEngine;

public class ProductionDataManager : IDataLoader<List<ProductionDef>>
{
	public Dictionary<int, ProductionDef> Production;

	public void LoadData(IDataMapper<List<ProductionDef>> dataMapper)
	{
		dataMapper.LoadData((data, error) =>
		{
			if (string.IsNullOrEmpty(error))
			{
				Production = new Dictionary<int, ProductionDef>();

				foreach (var def in data)
				{
					Production.Add(PieceType.Parse(def.Uid), def);
				}
			}
			else
			{
				Debug.LogWarning("[ProductionDataManager]: production config not loaded");
			}
		});
	}

	public ProductionDef GetProduction(int key)
	{
		ProductionDef def;
		return Production.TryGetValue(key, out def) ? def : null;
	}
}