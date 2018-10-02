using System.Collections.Generic;
using UnityEngine;

public class ShopDataManager : IECSComponent, IDataManager, IDataLoader<List<ShopDef>>
{
	public static int ComponentGuid = ECSManager.GetNextGuid();
	public int Guid => ComponentGuid;

	public Dictionary<string, List<ShopDef>> Defs;

	public void OnRegisterEntity(ECSEntity entity)
	{
		Reload();
	}

	public void OnUnRegisterEntity(ECSEntity entity)
	{
	}
	
	public void Reload()
	{
		Defs = new Dictionary<string, List<ShopDef>>();
		LoadData(new ResourceConfigDataMapper<List<ShopDef>>("configs/shop.data", NSConfigsSettings.Instance.IsUseEncryption));
	}

	public void LoadData(IDataMapper<List<ShopDef>> dataMapper)
	{
		dataMapper.LoadData((data, error) =>
		{
			if (string.IsNullOrEmpty(error))
			{
				foreach (var def in data)
				{
					List<ShopDef> list;

					if (Defs.TryGetValue(def.Uid, out list) == false)
					{
						list = new List<ShopDef>();
						Defs.Add(def.Uid, list);
					}
					
					list.Add(def);
				}
			}
			else
			{
				Debug.LogWarningFormat("[{0}]: config not loaded", GetType());
			}
		});
	}
}