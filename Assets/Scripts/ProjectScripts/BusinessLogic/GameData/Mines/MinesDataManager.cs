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
	
	public Dictionary<BoardPosition, MineDef> All;
	
	public List<BoardPosition> Moved = new List<BoardPosition>();
	public List<int> Removed = new List<int>();
	
	public void Reload()
	{
		All = null;
		Moved = new List<BoardPosition>();
		Removed = new List<int>();
		LoadData(new ResourceConfigDataMapper<List<MineDef>>("configs/mines.data", NSConfigsSettings.Instance.IsUseEncryption));
	}
	
	public void LoadData(IDataMapper<List<MineDef>> dataMapper)
	{
		All = new Dictionary<BoardPosition, MineDef>();
		
		dataMapper.LoadData((data, error) =>
		{
			if (string.IsNullOrEmpty(error))
			{
				var save = ProfileService.Current.GetComponent<FieldDefComponent>(FieldDefComponent.ComponentGuid);

				if (save != null)
				{
					if(save.MovedMinePositions != null) Moved = save.MovedMinePositions;
					if(save.RemovedMinePositions != null) Removed = save.RemovedMinePositions;
				}

				for (var i = data.Count - 1; i >= 0; i--)
				{
					var def = data[i];
					
					if (Removed.FindIndex(id => id == def.Id) != -1)
					{
						data.RemoveAt(i);
						continue;
					}

					for (var j = 0; j < Moved.Count; j++)
					{
						var move = Moved[j];
						if (move.Z != def.Id) continue;
						
						move.Z = 0;
						def.Position = move;
						break;
					}
					
					All.Add(def.Position, def);
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
		return All.TryGetValue(key, out def) == false ? null : def;
	}

	public void Chenge(int id, BoardPosition position)
	{
		position.Z = id;
		
		for (var i = Moved.Count - 1; i >= 0; i--)
		{
			var move = Moved[i];
			if (move.Z != id) continue;
			
			Moved[i] = position;
			return;
		}
		
		Moved.Add(position);
	}

	public void Remove(int id)
	{
		Removed.Add(id);

		for (var i = Moved.Count - 1; i >= 0; i--)
		{
			var move = Moved[i];
			if (move.Z != id) continue;
			
			Moved.RemoveAt(i);
			break;
		}
	}

	public int GetMineById(string id)
	{
		foreach (var def in All.Values)
		{
			if (def.Uid != id) continue;

			return PieceType.Parse(def.Skin);
		}

		return PieceType.None.Id;
	}
}