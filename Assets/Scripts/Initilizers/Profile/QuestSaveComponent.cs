using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[JsonObject(MemberSerialization.OptIn)]
public class QuestSaveComponent : ECSEntity, IECSSerializeable
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    [JsonProperty] public List<string> FinishedQuests;
    [JsonProperty] public List<QuestSaveData> ActiveQuests;

    [OnSerializing]
    internal void OnSerialization(StreamingContext context)
    {
        if(GameDataService.Current == null) return;
        
        Update();
    }
    
    private void Update()
    {
        ActiveQuests = GameDataService.Current.QuestsManager.GetQuestsSaveData();
        FinishedQuests = GameDataService.Current.QuestsManager.FinishedQuests;
    }
}

[JsonObject(MemberSerialization.OptIn)]
public class QuestSaveDataProxy
{
    [JsonProperty] public QuestEntity Quest;
    [JsonProperty] public List<TaskEntity> Tasks;
}

[JsonObject(MemberSerialization.OptIn)]
public class QuestSaveData
{
    public QuestEntity Quest;
    public List<TaskEntity> Tasks;

    [JsonProperty] public JToken Data ;
    
    [OnSerializing]
    internal void OnSerialization(StreamingContext context)
    {
        QuestSaveDataProxy proxy = new QuestSaveDataProxy {Quest = Quest, Tasks = Tasks};
        Data = JToken.FromObject(proxy);
    }
}