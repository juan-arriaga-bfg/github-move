using Debug = IW.Logger;
using System.Collections.Generic;
using UnityEngine;

public class PiecesDataManager : SequenceData, IDataLoader<List<PieceDef>>
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;
    
    private Dictionary<int, PieceDef> pieces;

    public override void OnRegisterEntity(ECSEntity entity)
    {
        base.OnRegisterEntity(entity);
        RegisterComponent(new PiecesReproductionDataManager());
        RegisterComponent(new PiecesMakingDataManager());
        RegisterComponent(new PiecesMineDataManager());
    }
    
    public override void Reload()
    {
        base.Reload();
        pieces = null;
        
        LoadData(new HybridConfigDataMapper<List<PieceDef>>("configs/pieces.data", NSConfigsSettings.Instance.IsUseEncryption));

        foreach (var component in componentsCache.Values)
        {
            var manager = component as IDataManager;
            manager?.Reload();
        }
    }

    public void LoadData(IDataMapper<List<PieceDef>> dataMapper)
    {
        dataMapper.LoadData((data, error)=>
        {
            if (string.IsNullOrEmpty(error))
            {
                pieces = new Dictionary<int, PieceDef>();

                foreach (var def in data)
                {
                    if (pieces.ContainsKey(def.Id)) continue;

                    AssignFilters(def);
                    pieces.Add(def.Id, def);
                }
            }
            else
            {
                Debug.LogWarningFormat("[{0}]: config not loaded", GetType());
            }
        });
    }

    private void AssignFilters(PieceDef pieceDef)
    {
        /*PieceTypeDef pieceTypeDef = PieceType.GetDefById(pieceDef.Id);

        if (pieceDef.SpawnResources != null && pieceDef.SpawnResources.Currency == Currency.Energy.Name)
        {
            pieceTypeDef.Filter = pieceTypeDef.Filter.Add(PieceTypeFilter.Energy);
        }*/
    }

    public PieceDef GetPieceDef(int id)
    {
        return pieces.TryGetValue(id, out var def) ? def : null;
    }
}
