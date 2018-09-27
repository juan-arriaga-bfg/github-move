using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public enum QuestStarterMode
{
    Once,
    Loop,
    Restart
}

[JsonObject(MemberSerialization.OptIn)]
public class QuestStarterEntity : ECSEntity, IECSSerializeable
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    
    public override int Guid => ComponentGuid;

    [JsonProperty] public string Id { get; protected set; }
    
    [JsonProperty] public QuestStarterMode Mode { get; protected set; }
    
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
        bool isTimeToStart = true;
        
        string log = $"[QuestStarterEntity] => Check: Starter Id: {Id}";

        for (var i = 0; i < conditions.Count; i++)
        {
            var condition = conditions[i];
            var result    = condition.Check();
            log += "\n" + $"Condition {i + 1}/{conditions.Count}: {condition.Id}, Result: {result}";

            if (!result)
            {
                isTimeToStart = false;
                break;
            }
        }

        log += "\n" + $"Resolution: {(isTimeToStart ? "START quest with id: " + QuestToStartId : "SKIP")}";
        Debug.Log(log);
        
        return isTimeToStart;
    }
}