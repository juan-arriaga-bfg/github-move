using System.Collections.Generic;
using UnityEngine;

public class PiecesDataManager : ECSEntity, IDataManager, IDataLoader<List<PieceDef>>
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    public override void OnRegisterEntity(ECSEntity entity)
    {
        Reload();
        RegisterComponent(new PiecesMatchConditionsManager());
        RegisterComponent(new PiecesReproductionDataManager());
    }
    
    public const int CreateManaDelay = 60;
    public const int ReproductionDelay = 20;
    public const int ReproductionStepDelay = 5;
    public const int ReproductionChance = 50;
    
    private Dictionary<int, PieceDef> pieces;
    
    public void Reload()
    {
        pieces = null;
        
        LoadData(new ResourceConfigDataMapper<List<PieceDef>>("configs/pieces.data", NSConfigsSettings.Instance.IsUseEncryption));
        
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
        PieceTypeDef pieceTypeDef = PieceType.GetDefById(pieceDef.Id);
        
        if (pieceDef.SpawnResources != null && pieceDef.SpawnResources.Currency == Currency.Energy.Name)
        {
            pieceTypeDef.Filter = pieceTypeDef.Filter.Add(PieceTypeFilter.Energy);
            // Debug.Log($"Add Energy filter to {pieceTypeDef.Abbreviations[0]}");
        }
    }

    public PieceDef GetPieceDef(int id)
    {
        PieceDef def;
        
        return pieces.TryGetValue(id, out def) ? def : null;
    }

    public PieceDef GetPieceDefOrDefault(int id)
    {
        PieceDef def;
        
        return pieces.TryGetValue(id, out def) ? def : PieceDef.Default();
    }
}