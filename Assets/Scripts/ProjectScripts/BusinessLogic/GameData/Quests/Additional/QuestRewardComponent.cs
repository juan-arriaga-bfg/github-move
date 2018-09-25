using System.Collections.Generic;
using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class QuestRewardComponent : IECSComponent, IECSSerializeable
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;
    
    [JsonProperty] public List<CurrencyPair> Value { get; protected set; }
    
    public void OnRegisterEntity(ECSEntity entity)
    {
        Value = new List<CurrencyPair> {new CurrencyPair {Amount = 100, Currency = Currency.Coins.Name}};
    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {
        
    }
}