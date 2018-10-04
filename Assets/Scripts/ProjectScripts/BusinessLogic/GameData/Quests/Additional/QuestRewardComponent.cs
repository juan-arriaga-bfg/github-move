using System.Collections.Generic;
using Newtonsoft.Json;

/// <summary>
/// Contains reward for Quest or Task
/// </summary>
[JsonObject(MemberSerialization.OptIn)]
public class QuestRewardComponent : IECSComponent, IECSSerializeable
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;
    
    [JsonProperty] public List<CurrencyPair> Value { get; protected set; }
    
    public void OnRegisterEntity(ECSEntity entity)
    {

    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {
        
    }
}