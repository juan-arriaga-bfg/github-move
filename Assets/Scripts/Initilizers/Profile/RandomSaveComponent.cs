using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class RandomSaveComponent : ECSEntity, IECSSerializeable
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;
    
    private List<RandomSaveItem> sequences;
    private Dictionary<string, RandomSaveItem> data;

    [JsonProperty]
    public List<RandomSaveItem> Sequences
    {
        get { return sequences; }
        set { sequences = value; }
    }
    
    [OnSerializing]
    internal void OnSerialization(StreamingContext context)
    {
        var cache = GameDataService.Current.ComponentsCache;
        sequences = new List<RandomSaveItem>();

        foreach (var component in cache.Values)
        {
            var seq = component as ISequenceData;
            
            if(seq == null) continue;
            
            sequences.AddRange(seq.GetSaveSequences());
        }
    }
    
    [OnDeserialized]
    internal void OnDeserialized(StreamingContext context)
    {
        data = new Dictionary<string, RandomSaveItem>();

        foreach (var sequence in sequences)
        {
            data.Add(sequence.Uid, sequence);
        }
    }

    public RandomSaveItem GetSave(string uid)
    {
        RandomSaveItem save;

        if (data == null || data.TryGetValue(uid, out save) == false) return null;

        data.Remove(uid);
        
        return save;
    }
}