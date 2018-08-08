using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class CodexSaveComponent : ECSEntity, IECSSerializeable
{
    public static int ComponentGuid = ECSManager.GetNextGuid();

    public override int Guid => ComponentGuid;

    [JsonProperty]
    public List<CodexDef> Data;

    [OnSerializing]
    internal void OnSerialization(StreamingContext context)
    {
        Data = GameDataService.Current?.CodexManager?.Items ?? new List<CodexDef>();
    }

    [OnDeserialized]
    internal void OnDeserialized(StreamingContext context)
    {
    }
}