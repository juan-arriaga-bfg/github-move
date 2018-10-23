using System.Collections.Generic;

public abstract class SequenceData : ECSEntity, IDataManager
{
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
        var sequenceData = new SequenceComponent{Key = uid};
                    
        sequenceData.Init(weights);
        RegisterComponent(sequenceData, true);
    }
    
    public SequenceComponent GetSequence(string uid)
    {
        var collection = GetComponent<ECSComponentCollection>(SequenceComponent.ComponentGuid);
        return (SequenceComponent) collection.Components.Find(component =>
        {
            var sequenceData = component as SequenceComponent;
            
            return sequenceData != null && sequenceData.Key == uid;
        });
    }

    public abstract void UpdateSequence();
}