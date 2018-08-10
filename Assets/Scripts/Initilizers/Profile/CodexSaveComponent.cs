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
    
    [JsonProperty("State")]
    public CodexState State;

    [OnSerializing]
    internal void OnSerialization(StreamingContext context)
    {
        var codexManager = GameDataService.Current?.CodexManager;
        if (codexManager == null)
        {
            return;
        }
        
        var items = codexManager.Items ?? new Dictionary<int, CodexChainState>();
        Data = items;

        State = codexManager.CodexState;
    }
    
    //
    // [OnDeserialized]
    // internal void OnDeserialized(StreamingContext context)
    // {
    // }
}