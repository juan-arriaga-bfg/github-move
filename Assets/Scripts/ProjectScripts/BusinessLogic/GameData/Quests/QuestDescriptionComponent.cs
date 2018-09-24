using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class QuestDescriptionComponent : IECSComponent, IECSSerializeable
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;

    [JsonProperty] public string Message { get; protected set; }
    [JsonProperty] public string Ico { get; protected set; }
    
    public void OnRegisterEntity(ECSEntity entity)
    {
        Message = "test mess";
        Ico = "test ico";
    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {
        
    }
}