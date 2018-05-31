using System.Collections.Generic;
using UnityEngine;

public class CollectionDataManager : IDataLoader<CollectionDataManager>
{
	public int Chance;
	public List<CollectionDef> Collection;
	
	public Dictionary<BoardPosition, List<CurrencyPair>> ResourcesOnBoard = new Dictionary<BoardPosition, List<CurrencyPair>>();
    
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
				Debug.LogWarning("[CollectionDataManager]: collection config not loaded");
			}
		});
	}
	
	public bool Contains(string name)
	{
		var item = Collection.Find(def => def.Uid == name);

		return item != null;
	}

	public string GetRandomItem()
	{
		var level = ProfileService.Current.GetStorageItem(Currency.Level.Name).Amount;
		var hot = new List<ItemWeight>();
		var top = new List<ItemWeight>();

		foreach (var def in Collection)
		{
			if (ProfileService.Current.GetStorageItem(def.Uid).Amount > 0)
			{
				continue;
			}
			
			if (def.MaxLevel <= level)
			{
				if (def.Ignore) return def.Uid;
				
				hot.Add(def.GetItemWeight());
				continue;
			}

			if (def.MinLevel <= level)
			{
				top.Add(def.GetItemWeight());
			}
		}
		
		var random = Random.Range(0, 101);

		if (random > Chance) return null;
		
		if (hot.Count != 0) return ItemWeight.GetRandomItem(hot).Uid;
		if (top.Count != 0) return ItemWeight.GetRandomItem(top).Uid;
		
		return null;
	}

	public void CastResourceOnBoard(BoardPosition at, CurrencyPair resource)
	{
		List<CurrencyPair> resources;

		if (ResourcesOnBoard.TryGetValue(at, out resources) == false)
		{
			resources = new List<CurrencyPair>();
			ResourcesOnBoard.Add(at, resources);
		}
		
		resources.Add(resource);
		ResourceView.Show(at, resource);
	}

	public void CollectResourceFromBoard(BoardPosition at, CurrencyPair resource)
	{
		List<CurrencyPair> resources;

		if (ResourcesOnBoard.TryGetValue(at, out resources) == false) return;

		var item = resources.Find(pair => pair.Currency == resource.Currency && pair.Amount == resource.Amount);

		if (item == null) return;
		
		CurrencyHellper.Purchase(resource);
		BoardService.Current.GetBoardById(0).ProductionLogic.Update();

		resources.Remove(item);
		
		if(resources.Count != 0) return;

		ResourcesOnBoard.Remove(at);
	}
}