﻿using System.Collections.Generic;
using UnityEngine;

public class ShopDataManager : IECSComponent, IDataManager, IDataLoader<List<ShopDef>>
{
	public static int ComponentGuid = ECSManager.GetNextGuid();

	public int Guid { get { return ComponentGuid; } }

	public List<ShopDef> Products = new List<ShopDef>();
	
	public void OnRegisterEntity(ECSEntity entity)
	{
		Reload();
	}
    
	public void OnUnRegisterEntity(ECSEntity entity)
	{
	}
	
	public void Reload()
	{
		Products = new List<ShopDef>();
		LoadData(new ResourceConfigDataMapper<List<ShopDef>>("configs/shop.data", NSConfigsSettings.Instance.IsUseEncryption));
	}
    
	public void LoadData(IDataMapper<List<ShopDef>> dataMapper)
	{
		dataMapper.LoadData((data, error)=> 
		{
			if (string.IsNullOrEmpty(error))
			{
				Products = data;
			}
			else
			{
				Debug.LogWarningFormat("[{0}]: config not loaded", GetType());
			}
		});
	}
}