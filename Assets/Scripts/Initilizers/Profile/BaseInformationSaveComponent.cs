using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class BaseInformationSaveComponent : ECSEntity, IECSSerializeable
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;
	
    private DateTime creationDateTime = DateTime.UtcNow;
    [JsonProperty]
    public DateTime CreationDateTime
    {
        get { return creationDateTime; }
        set { creationDateTime = value; }
    }

    private bool isPayer = false;
    [JsonProperty]
    public bool IsPayer
    {
        get { return isPayer; }
        set { isPayer = value; }
    }

    [OnSerializing]
    internal void OnSerialization(StreamingContext context)
    {
    }

    [OnDeserialized]
    internal void OnDeserialized(StreamingContext context)
    {
    }
}