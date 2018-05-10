using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;

[JsonObject(MemberSerialization.OptIn)]
public class QuestSaveComponent : ECSEntity, IECSSerializeable
{
    public static int ComponentGuid = ECSManager.GetNextGuid();

    public override int Guid
    {
        get { return ComponentGuid; }
    }

    private List<QuestSaveItem> active;
    private List<int> completed;

    [JsonProperty]
    public List<QuestSaveItem> Active
    {
        get { return active; }
        set { active = value; }
    }
    
    [JsonProperty]
    public List<int> Completed
    {
        get { return completed; }
        set { completed = value; }
    }
    
    [OnSerializing]
    internal void OnSerialization(StreamingContext context)
    {
        if(GameDataService.Current == null) return;

        var manager = GameDataService.Current.QuestsManager;
        
        completed = manager.SaveCompleted();
        completed.Sort((a, b) => a.CompareTo(b));
        
        active = new List<QuestSaveItem>();

        foreach (var quest in manager.ActiveQuests)
        {
            active.Add(new QuestSaveItem{Uid = quest.Def.Uid, Progress = quest.CurrentAmount});
        }
        
        active.Sort((a, b) => a.Uid.CompareTo(b.Uid));
    }
}