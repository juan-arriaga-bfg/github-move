using System;
using Newtonsoft.Json;

public class FogSaveItemJsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof (FogSaveItem);
    }
    
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var targetValue = (FogSaveItem) value;
        
        serializer.TypeNameHandling = TypeNameHandling.None;
        serializer.Serialize(writer, $"{targetValue.AlreadyPaid},{targetValue.Uid}");
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var data = serializer.Deserialize<string>(reader);
        var dataArray = data.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
        
        var targetValue = new FogSaveItem
        {
            AlreadyPaid = int.Parse(dataArray[0]),
            Uid = dataArray[1]
        };

        // Migration - load old version when we have positions instead uid here
        if (dataArray.Length > 2)
        {
            for (int i = 2; i < dataArray.Length; i++)
            {
                targetValue.Uid += ("," + dataArray[i]);
            }
        }
        
        return targetValue;
    }
}

[JsonConverter(typeof(FogSaveItemJsonConverter))]
public class FogSaveItem
{
    public string Uid;

    public int AlreadyPaid;
}