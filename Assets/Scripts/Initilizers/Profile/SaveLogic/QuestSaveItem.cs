using System;
using Newtonsoft.Json;

public class QuestSaveItemJsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof (QuestSaveItem);
    }
    
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var targetValue = (QuestSaveItem) value;
        
        serializer.TypeNameHandling = TypeNameHandling.None;

        serializer.Serialize(writer, string.Format("{0},{1}",
            targetValue.Uid,
            targetValue.Progress));
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var data = serializer.Deserialize<string>(reader);
        var dataArray = data.Split(new string[] {","}, StringSplitOptions.RemoveEmptyEntries);
        
        var targetValue = new QuestSaveItem
        {
            Uid = int.Parse(dataArray[0]),
            Progress = int.Parse(dataArray[1])
        };
        
        return targetValue;
    }
}

[JsonConverter(typeof(QuestSaveItemJsonConverter))]
public class QuestSaveItem
{
    private int uid;
    private int progress;

    public int Uid
    {
        get { return uid; }
        set { uid = value; }
    }

    public int Progress
    {
        get { return progress; }
        set { progress = value; }
    }
}