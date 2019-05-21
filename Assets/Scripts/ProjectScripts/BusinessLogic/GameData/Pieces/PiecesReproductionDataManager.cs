using Debug = IW.Logger;
using System.Collections.Generic;
using UnityEngine;

public class PiecesReproductionDataManager : IECSComponent, IDataManager, IDataLoader<List<PieceReproductionDef>>
{
	public static int ComponentGuid = ECSManager.GetNextGuid();
	public int Guid => ComponentGuid;
	
	private PiecesDataManager context;
    
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
		LoadData(new HybridConfigDataMapper<List<PieceReproductionDef>>("configs/piecesReproduction.data", NSConfigsSettings.Instance.IsUseEncryption));
	}

	public void LoadData(IDataMapper<List<PieceReproductionDef>> dataMapper)
	{
		dataMapper.LoadData((data, error)=> 
		{
			if (string.IsNullOrEmpty(error))
			{
				foreach (var def in data)
				{
					var baseDef = context.GetPieceDef(def.Id);
                    
					if(baseDef == null) continue;

					baseDef.ReproductionDef = def;
					
					var typeDef = PieceType.GetDefById(def.Id);
					typeDef.Filter = typeDef.Filter.Add(PieceTypeFilter.Reproduction);
				}
			}
			else
			{
				Debug.LogWarningFormat("[{0}]: config not loaded", GetType());
			}
		});
	}
}