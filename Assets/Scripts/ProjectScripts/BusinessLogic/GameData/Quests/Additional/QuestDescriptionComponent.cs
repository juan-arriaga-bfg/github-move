using Newtonsoft.Json;

/// <summary>
/// Contains user friendly description of Quest or Task
/// </summary>
[JsonObject(MemberSerialization.OptIn)]
public class QuestDescriptionComponent : IECSComponent, IECSSerializeable
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;

    [JsonProperty] public string Title { get; protected set; }
    [JsonProperty] public string Message { get; protected set; }
    [JsonProperty] public string Ico { get; protected set; }
    
    public void OnRegisterEntity(ECSEntity entity)
    {

    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {
        
    }
}