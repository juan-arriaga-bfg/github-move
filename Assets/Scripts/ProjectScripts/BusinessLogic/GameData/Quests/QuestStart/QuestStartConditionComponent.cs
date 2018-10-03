using Newtonsoft.Json;

public abstract class QuestStartConditionComponent : IECSComponent, IECSSerializeable
{    
    public abstract int Guid { get; }
    
    [JsonProperty] public string Id { get; protected set; }
    
    public void OnRegisterEntity(ECSEntity entity)
    {

    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {

    }

    public abstract bool Check();
}