using System.Collections.Generic;

public abstract class SequenceData : ECSEntity, IDataManager
{
    public ECSEntity context;
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        context = entity;
        
        Reload();
    }
    
    public override void OnUnRegisterEntity(ECSEntity entity)
    {
        context = null;
    }
    
    public virtual void Reload()
    {
        UnRegisterSequences();
    }
    
    public List<SequenceSaveItem> GetSaveSequences()
    {
        var save = new List<SequenceSaveItem>();
        var collection = GetComponent<ECSComponentCollection>(SequenceComponent.ComponentGuid);

        if (collection?.Components == null) return save;

        var components = collection.Components.FindAll(component => component is SequenceComponent);
        
        foreach (SequenceComponent component in components)
        {
            save.Add(component.Save());
        }
        
        return save;
    }

    private void UnRegisterSequences()
    {
        var collection = GetComponent<ECSComponentCollection>(SequenceComponent.ComponentGuid);

        if (collection?.Components == null) return;
        
        var components = new List<IECSComponent>(collection.Components);
            
        foreach (var component in components)
        {
            UnRegisterComponent(component);
        }
    }

    public void AddSequence(string uid, List<ItemWeight> weights)
    {
        var sequenceData = new SequenceComponent
        {
            Key = uid, 
            Context = context
        };
        
        RegisterComponent(sequenceData, true);
        sequenceData.Init(weights);
    }
    
    public SequenceComponent GetSequence(string uid)
    {
        var collection = GetComponent<ECSComponentCollection>(SequenceComponent.ComponentGuid);
        return (SequenceComponent) collection.Components.Find(component => component is SequenceComponent sequenceData && sequenceData.Key == uid);
    }
}