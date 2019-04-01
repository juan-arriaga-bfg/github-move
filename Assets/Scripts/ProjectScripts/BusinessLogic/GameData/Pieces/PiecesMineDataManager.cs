using Debug = IW.Logger;
using System.Collections.Generic;
using UnityEngine;

public class PiecesMineDataManager : IECSComponent, IDataManager, IDataLoader<List<PieceMineDef>>
{
	public static int ComponentGuid = ECSManager.GetNextGuid();
	public int Guid => ComponentGuid;
	
	private PiecesDataManager context;

	private List<LoopSaveItem> loops;
    
	public void OnRegisterEntity(ECSEntity entity)
	{
		context = entity as PiecesDataManager;
		Reload();
	}

	public void OnUnRegisterEntity(ECSEntity entity)
	{
	}
	
	public void Reload()
	{
		LoadData(new ResourceConfigDataMapper<List<PieceMineDef>>("configs/piecesMine.data", NSConfigsSettings.Instance.IsUseEncryption));
	}
	
	public void LoadData(IDataMapper<List<PieceMineDef>> dataMapper)
	{
		dataMapper.LoadData((data, error)=> 
		{
			var save = ((GameDataManager)context.context).UserProfile.FieldDef;
			if (save.Loops == null) save.Loops = new List<LoopSaveItem>();

			loops = save.Loops;
			
			if (string.IsNullOrEmpty(error))
			{
				foreach (var def in data)
				{
					var baseDef = context.GetPieceDef(def.Id);

					if (baseDef == null) continue;

					baseDef.MineDef = def;

					var saveItem = loops.Find(item => item.Uid == def.Id);

					if (saveItem != null) continue;

					loops.Add(new LoopSaveItem {Uid = def.Id, Value = def.Loop});
				}
			}
			else
			{
				Debug.LogWarningFormat("[{0}]: config not loaded", GetType());
			}
		});
	}

	public void DecrementLoop(int uid)
	{
		var item = loops.Find(save => save.Uid == uid);

		if (item == null || item.Value <= 0) return;

		item.Value -= 1;
	}

	public int GetCurrentLoop(int uid)
	{
		var item = loops.Find(save => save.Uid == uid);
		
		return item?.Value ?? 0;
	}
	
}