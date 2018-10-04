using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public enum QuestStarterMode
{
    Once,    // Quest may run only once
    Loop,    // Quest may run again after completion
    Restart  // Quest will be restarted even it already in progress right now
}

/// <summary>
/// Starter will call Check method of all added Conditions and return true if all Conditions are true.
/// </summary>
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
        string log = $"[QuestStarterEntity] => Check: Starter Id: {Id}";
        
        bool isTimeToStart = true;

        var dataManager = GameDataService.Current.QuestsManager;
        
        switch (Mode)
        {
            case QuestStarterMode.Once:
                if (dataManager.CompletedQuests.Contains(QuestToStartId))
                {
                    isTimeToStart = false;
                    log += "\n" + $"QuestStarterMode is 'Once' but quest have been completed.";
                }
                else if (dataManager.GetActiveQuestById(QuestToStartId) != null)
                {
                    isTimeToStart = false;
                    log += "\n" + $"QuestStarterMode is 'Once' but quest already in progress.";
                }
                break;
            
            case QuestStarterMode.Loop:
                throw new NotImplementedException();
                break;
            
            case QuestStarterMode.Restart:
                throw new NotImplementedException();
                break;
        }


        if (isTimeToStart)
        {
            for (var i = 0; i < conditions.Count; i++)
            {
                var condition = conditions[i];
                var result    = condition.Check();
                log += "\n" + $"Condition {i + 1}/{conditions.Count}: id: {condition.Id}, Result: {result}";

                if (!result)
                {
                    isTimeToStart = false;
                    break;
                }
            }
        }

        log += "\n" + $"Resolution: {(isTimeToStart ? "START quest with id: " + QuestToStartId : "SKIP")}";
        Debug.Log(log);
        
        return isTimeToStart;
    }
}