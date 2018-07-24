using System.Collections.Generic;
using UnityEngine;

public class ProductionDataManager : IECSComponent, IDataManager, IDataLoader<List<ProductionDef>>
{
	public static int ComponentGuid = ECSManager.GetNextGuid();

	public int Guid { get { return ComponentGuid; } }
	
	public void OnRegisterEntity(ECSEntity entity)
	{
		Reload();
	}

	public void OnUnRegisterEntity(ECSEntity entity)
	{
	}
	
	public Dictionary<int, ProductionDef> Production;
	
	public void Reload()
	{
		Production = null;
		LoadData(new ResourceConfigDataMapper<List<ProductionDef>>("configs/production.data", NSConfigsSettings.Instance.IsUseEncryption));
	}

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
				Debug.LogWarningFormat("[{0}]: config not loaded", GetType());
			}
		});
	}

	public ProductionDef GetProduction(int key)
	{
		ProductionDef def;
		return Production.TryGetValue(key, out def) ? def : null;
	}
}