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

    [JsonProperty] public List<string> CompletedQuests;
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
        CompletedQuests = GameDataService.Current.QuestsManager.CompletedQuests;
    }
}

[JsonObject(MemberSerialization.OptIn)]
public class QuestSaveDataProxy
{
    [JsonProperty] public QuestEntity Quest;
    [JsonProperty] public List<TaskEntity> Tasks;
}

// [JsonConverter(typeof(QuestSaveDataJsonConverter))]
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
//
// /// <summary>
// /// Warning! Not thread safe!
// /// </summary>
// public class QuestSaveDataJsonConverter : JsonConverter
// {
//     // Disable converter for writing to avoid infinite loop
//     private bool allowed = true;
//     
//     public override bool CanConvert(Type objectType)
//     {
//         return objectType == typeof (QuestSaveData);
//     }
//
//     public override bool CanWrite => allowed;
//
//     public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
//     {
//         var targetValue = (QuestSaveData) value;
//         
//         serializer.TypeNameHandling = TypeNameHandling.None;
//
//         allowed = false;
//         JsonSerializer.CreateDefault().Serialize(writer, targetValue);
//         allowed = true;
//     }
//
//     public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
//     {
//         StringBuilder sb = new StringBuilder();
//         while (reader.Read())
//         {
//             sb.Append(reader.Value);
//         }
//
//         var ret = new QuestSaveData
//         {
//             DataAsJson = sb.ToString()
//         };
//         return ret;
//
//         // var data      = serializer.Deserialize<string>(reader);
//         // var dataArray = data.Split(new string[] {","}, StringSplitOptions.RemoveEmptyEntries);
//         //
//         // var targetValue = new QuestSaveItem
//         // {
//         //     Uid = int.Parse(dataArray[0]),
//         //     Progress = int.Parse(dataArray[1])
//         // };
//         //
//         // return targetValue;
//     }
// }