using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class SequenceSaveComponent : ECSEntity, IECSSerializeable, IProfileSaveComponent
{
    public bool AllowDataCollect { get; set; }

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
        if (!AllowDataCollect)
        {
            return;
        }
        
        var cache = GameDataService.Current.ComponentsCache;
        sequences = new List<SequenceSaveItem>();

        foreach (var component in cache.Values)
        {
            if(!(component is SequenceData seq)) continue;
            
            sequences.AddRange(seq.GetSaveSequences());
        }
    }
    
    [OnDeserialized]
    internal void OnDeserialized(StreamingContext context)
    {
        data = new Dictionary<string, SequenceSaveItem>();

        if (DevTools.IsSequenceReset)
        {
            DevTools.IsSequenceReset = false;
            return;
        }

        foreach (var sequence in sequences)
        {
            data.Add(sequence.Uid, sequence);
        }
    }

    public SequenceSaveItem GetSave(string uid)
    {
        if (data == null || data.TryGetValue(uid, out var save) == false) return null;

        data.Remove(uid);
        
        return save;
    }
}