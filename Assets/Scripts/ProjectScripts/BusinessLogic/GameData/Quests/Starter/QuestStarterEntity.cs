using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
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
    
    [JsonProperty] public List<string> QuestToStartIds { get; protected set; }
    
    [JsonProperty] public List<QuestStartConditionComponent> Conditions = new List<QuestStartConditionComponent>();
    
#region Serialization
    
    public bool ShouldSerializeConditions()
    {
        return false;
    }
    
#endregion
    
    public bool Check()
    {
        StringBuilder log = new StringBuilder($"[QuestStarterEntity] => Check: Starter Id: {Id}");
        
        bool isTimeToStart = true;

        var dataManager = GameDataService.Current.QuestsManager;

        foreach (var questId in QuestToStartIds)
        {
            if (isTimeToStart == false)
            {
                break;
            }
            
            switch (Mode)
            {
                case QuestStarterMode.Once:
                    if (dataManager.CompletedQuests.Contains(questId))
                    {
                        isTimeToStart = false;
                        log.Append("\n" + $"QuestStarterMode is 'Once' but quest have been completed.");
                    }
                    else if (dataManager.GetActiveQuestById(questId) != null)
                    {
                        isTimeToStart = false;
                        log.Append("\n" + $"QuestStarterMode is 'Once' but quest already in progress.");
                    }
                    break;
            
                case QuestStarterMode.Loop:
                    throw new NotImplementedException();
                    break;
            
                case QuestStarterMode.Restart:
                    throw new NotImplementedException();
                    break;
            }
        }

        if (isTimeToStart)
        {
            for (var i = 0; i < Conditions.Count; i++)
            {
                var condition = Conditions[i];
                bool result = condition.Check();
                log.Append("\n" + $"Condition {i + 1}/{Conditions.Count}: {condition.GetType().ToString().Replace("QuestStartCondition", "").Replace("Component", "")} [id: {condition.Id}], Result: {result}");

                if (!result)
                {
                    isTimeToStart = false;
                    break;
                }
            }
        }

        log.Append("\n" + $"Resolution: {(isTimeToStart ? $"START quests with ids: [{string.Join(",", QuestToStartIds)}]" : "SKIP")}");

        Debug.Log(log.ToString());
        
        return isTimeToStart;
    }
}