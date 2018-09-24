using Newtonsoft.Json;
using Quests;

public class TaskEntity : ECSEntity, IECSSerializeable
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    
    public override int Guid => ComponentGuid;

    [JsonProperty] public string Id;

    [JsonProperty] public TaskState State { get; protected set; }
}