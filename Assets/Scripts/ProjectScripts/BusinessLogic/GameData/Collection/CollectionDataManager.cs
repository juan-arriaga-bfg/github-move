using System.Collections.Generic;
using UnityEngine;

public class CollectionDataManager : IDataLoader<CollectionDataManager>
{
	public int Chance;
	public List<CollectionDef> Collection;
    
	public void LoadData(IDataMapper<CollectionDataManager> dataMapper)
	{
		dataMapper.LoadData((data, error)=> 
		{
			if (string.IsNullOrEmpty(error))
			{
				Chance = data.Chance;
				Collection = data.Collection;
			}
			else
			{
				Debug.LogWarning("[FogsDataManager]: fogs config not loaded");
			}
		});
	}

	public string GetRandomItem()
	{
		var random = Random.Range(0, 101);

		if (random < Chance) return null;

		var level = ProfileService.Current.GetStorageItem(Currency.Level.Name).Amount;
		var hot = new List<ItemWeight>();
		var top = new List<ItemWeight>();

		foreach (var def in Collection)
		{
			if (def.MaxLevel <= level)
			{
				hot.Add(def.GetItemWeight());
				continue;
			}

			if (def.MinLevel <= level)
			{
				top.Add(def.GetItemWeight());
			}
		}

		if (hot.Count != 0) return ItemWeight.GetRandomItem(hot).Uid;
		if (top.Count != 0) return ItemWeight.GetRandomItem(top).Uid;
		
		return null;
	}
}