using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class CodexSaveComponent : ECSEntity, IECSSerializeable
{
    public static int ComponentGuid = ECSManager.GetNextGuid();

    public override int Guid => ComponentGuid;

    [JsonProperty("Data")]
    public Dictionary<int, CodexChainState> Data;

    [OnSerializing]
    internal void OnSerialization(StreamingContext context)
    {
        var items = GameDataService.Current?.CodexManager?.Items ?? new Dictionary<int, CodexChainState>();
        Data = items;
    }
    //
    // [OnDeserialized]
    // internal void OnDeserialized(StreamingContext context)
    // {
    // }
}