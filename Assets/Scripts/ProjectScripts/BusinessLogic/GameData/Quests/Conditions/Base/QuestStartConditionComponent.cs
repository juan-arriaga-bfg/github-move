using Newtonsoft.Json;

/// <summary>
/// Base class for Start Conditions.
/// </summary>
public abstract class QuestStartConditionComponent : IECSComponent, IECSSerializeable
{    
    public abstract int Guid { get; }
    
    [JsonProperty] public string Id { get; protected set; }
    
#region Serialization

    [JsonProperty] public string Value;   
    
    public bool ShouldSerializeValue()
    {
        return false;
    }
    
#endregion
    
    public void OnRegisterEntity(ECSEntity entity)
    {

    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {

    }

    public abstract bool Check();
}