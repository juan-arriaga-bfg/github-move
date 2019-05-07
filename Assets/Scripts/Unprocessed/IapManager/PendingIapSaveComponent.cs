using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class PendingIapSaveComponent : ECSEntity, IECSSerializeable, IProfileSaveComponent
{
    public bool AllowDataCollect { get; set; }

    public static int ComponentGuid = ECSManager.GetNextGuid();

    public override int Guid => ComponentGuid;

    [JsonProperty] public Dictionary<string, PendingIap> PendingIaps;

    [OnSerializing]
    internal void OnSerialization(StreamingContext context)
    {
        if (!AllowDataCollect)
        {
            return;
        }
        
        PendingIaps = IapService.Current?.PendingIaps;
    }
}