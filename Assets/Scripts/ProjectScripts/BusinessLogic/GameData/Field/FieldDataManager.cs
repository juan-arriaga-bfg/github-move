using System.Collections.Generic;
using UnityEngine;

public class FieldDataManager : IECSComponent, IDataManager, IDataLoader<List<FieldDef>>
{
	public static int ComponentGuid = ECSManager.GetNextGuid();

	public int Guid
	{
		get { return ComponentGuid; }
	}

	public Dictionary<int, List<BoardPosition>> Pieces = new Dictionary<int, List<BoardPosition>>();

	public void OnRegisterEntity(ECSEntity entity)
	{
		Reload();
	}

	public void OnUnRegisterEntity(ECSEntity entity)
	{
	}

	public void Reload()
	{
		Pieces = new Dictionary<int, List<BoardPosition>>();
		LoadData(new ResourceConfigDataMapper<List<FieldDef>>("configs/field.data", NSConfigsSettings.Instance.IsUseEncryption));
	}
	
	public void LoadData(IDataMapper<List<FieldDef>> dataMapper)
	{
		dataMapper.LoadData((data, error)=> 
		{
			if (string.IsNullOrEmpty(error))
			{
				foreach (var def in data)
				{
					List<BoardPosition> list;

					if (Pieces.TryGetValue(def.Id, out list) == false)
					{
						list = new List<BoardPosition>();
						Pieces.Add(def.Id, list);
					}
					
					list.Add(def.Position);
				}
			}
			else
			{
				Debug.LogWarningFormat("[{0}]: config not loaded", GetType());
			}
		});
	}
}