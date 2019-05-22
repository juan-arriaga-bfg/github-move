using Debug = IW.Logger;
using System.Collections.Generic;
using UnityEngine;

public class ShopDataManager : IECSComponent, IDataManager, IDataLoader<List<ShopDef>>
{
	public static int ComponentGuid = ECSManager.GetNextGuid();
	public int Guid => ComponentGuid;
	
	private GameDataManager context;
	
	public Dictionary<string, List<ShopDef>> Defs;

	public void OnRegisterEntity(ECSEntity entity)
	{
		context = (GameDataManager)entity;
		
		Reload();
	}

	public void OnUnRegisterEntity(ECSEntity entity)
	{
		context = null;
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
				var userGroup = context.AbTestManager.Tests[AbTestName.SHOP_PRICE].UserGroup;
				
				foreach (var def in data)
				{
					if (string.IsNullOrEmpty(def.Cohort) == false && def.Cohort.ToLower() != userGroup) continue;
					
					if (Defs.TryGetValue(def.Uid, out var list) == false)
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