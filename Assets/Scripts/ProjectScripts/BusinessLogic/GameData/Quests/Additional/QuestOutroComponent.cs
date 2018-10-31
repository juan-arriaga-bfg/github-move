using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class QuestOutroComponent : IECSComponent, IECSSerializeable
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;

    [JsonProperty] public string Message { get; protected set; }
    
    public void OnRegisterEntity(ECSEntity entity)
    {

    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {
        
    }
}