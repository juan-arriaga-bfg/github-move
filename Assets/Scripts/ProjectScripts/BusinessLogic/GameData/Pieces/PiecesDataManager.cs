using System.Collections.Generic;
using UnityEngine;

public class PiecesDataManager : ECSEntity, IDataManager, IDataLoader<List<PieceDef>>, ISequenceData
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;
    
    private Dictionary<int, PieceDef> pieces;

    public override void OnRegisterEntity(ECSEntity entity)
    {
        Reload();
        RegisterComponent(new PiecesMatchConditionsManager());
        RegisterComponent(new PiecesReproductionDataManager());
        RegisterComponent(new PiecesMakingDataManager());
    }
    
    public List<RandomSaveItem> GetSaveSequences()
    {
        var save = new List<RandomSaveItem>();

        var collection = GetComponent<ECSComponentCollection>(SequenceComponent.ComponentGuid);

        foreach (SequenceComponent component in collection.Components)
        {
            save.Add(component.Save());
        }
        
        return save;
    }

    public void ReloadSequences()
    {
        var collection = GetComponent<ECSComponentCollection>(SequenceComponent.ComponentGuid);

        if (collection?.Components == null) return;
        
        var components = new List<IECSComponent>(collection.Components);
            
        foreach (var component in components)
        {
            UnRegisterComponent(component);
        }
    }
    
    public void Reload()
    {
        pieces = null;

        ReloadSequences();
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
        /*PieceTypeDef pieceTypeDef = PieceType.GetDefById(pieceDef.Id);

        if (pieceDef.SpawnResources != null && pieceDef.SpawnResources.Currency == Currency.Energy.Name)
        {
            pieceTypeDef.Filter = pieceTypeDef.Filter.Add(PieceTypeFilter.Energy);
        }*/
    }

    public PieceDef GetPieceDef(int id)
    {
        PieceDef def;

        return pieces.TryGetValue(id, out def) ? def : null;
    }
    
    public SequenceComponent GetSequence(string uid)
    {
        var collection = GetComponent<ECSComponentCollection>(SequenceComponent.ComponentGuid);
        return (SequenceComponent) collection.Components.Find(component => (component as SequenceComponent).Key == uid);
    }
}
