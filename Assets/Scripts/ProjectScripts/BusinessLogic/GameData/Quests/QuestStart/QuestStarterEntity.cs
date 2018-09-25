using System.Collections.Generic;
using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class QuestStarterEntity : ECSEntity, IECSSerializeable
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    
    public override int Guid => ComponentGuid;

    [JsonProperty] public string Id { get; protected set; }
    
    [JsonProperty] public List<string> ConditionIds { get; protected set; }
    
    private List<QuestStartConditionComponent> conditions = new List<QuestStartConditionComponent>();
    
    [JsonProperty] public string QuestToStartId { get; protected set; }
    
    public override ECSEntity RegisterComponent(IECSComponent component, bool isCollection = false)
    {
        var conditionComponent = component as QuestStartConditionComponent;
        if (conditionComponent != null)
        {
            conditions.Add(conditionComponent);
        }
        
        return base.RegisterComponent(component, isCollection);
    }
    
    public bool Check()
    {
        foreach (var condition in conditions)
        {
            if (!condition.Check())
            {
                return false;
            }
        }

        return false;
    }
}