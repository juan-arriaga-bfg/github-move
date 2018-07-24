using System.Collections.Generic;
using UnityEngine;

public class MinesDataManager : IECSComponent, IDataManager, IDataLoader<List<MineDef>>
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
	
	public Dictionary<BoardPosition, MineDef> MinePositions;
	
	public void Reload()
	{
		MinePositions = null;
		LoadData(new ResourceConfigDataMapper<List<MineDef>>("configs/mines.data", NSConfigsSettings.Instance.IsUseEncryption));
	}
	
	public void LoadData(IDataMapper<List<MineDef>> dataMapper)
	{
		MinePositions = new Dictionary<BoardPosition, MineDef>();
		
		dataMapper.LoadData((data, error) =>
		{
			if (string.IsNullOrEmpty(error))
			{
				foreach (var def in data)
				{
					MinePositions.Add(def.Position, def);
				}
			}
			else
			{
				Debug.LogWarningFormat("[{0}]: config not loaded", GetType());
			}
		});
	}
	
	public MineDef GetDef(BoardPosition key)
	{
		key.Z = 0;
		MineDef def;
		return MinePositions.TryGetValue(key, out def) == false ? null : def;
	}
}