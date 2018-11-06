using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class SequenceSaveComponent : ECSEntity, IECSSerializeable
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;
    
    private List<SequenceSaveItem> sequences;
    private Dictionary<string, SequenceSaveItem> data;

    [JsonProperty]
    public List<SequenceSaveItem> Sequences
    {
        get { return sequences; }
        set { sequences = value; }
    }
    
    [OnSerializing]
    internal void OnSerialization(StreamingContext context)
    {
        var cache = GameDataService.Current.ComponentsCache;
        sequences = new List<SequenceSaveItem>();

        foreach (var component in cache.Values)
        {
            var seq = component as SequenceData;
            
            if(seq == null) continue;
            
            sequences.AddRange(seq.GetSaveSequences());
        }
    }
    
    [OnDeserialized]
    internal void OnDeserialized(StreamingContext context)
    {
        data = new Dictionary<string, SequenceSaveItem>();

        foreach (var sequence in sequences)
        {
            data.Add(sequence.Uid, sequence);
        }
    }

    public SequenceSaveItem GetSave(string uid)
    {
        SequenceSaveItem save;

        if (data == null || data.TryGetValue(uid, out save) == false) return null;

        data.Remove(uid);
        
        return save;
    }
}