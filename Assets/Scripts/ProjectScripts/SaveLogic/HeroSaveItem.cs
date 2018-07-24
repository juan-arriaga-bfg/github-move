using System;
using Newtonsoft.Json;

public class HeroSaveItemJsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof (HeroSaveItem);
    }
    
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var targetValue = (HeroSaveItem) value;
        
        serializer.TypeNameHandling = TypeNameHandling.None;

        serializer.Serialize(writer, string.Format("{0},{1}",
            targetValue.Id,
            targetValue.StartTime.ConvertToUnixTime()));
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var data = serializer.Deserialize<string>(reader);
        var dataArray = data.Split(new string[] {","}, StringSplitOptions.RemoveEmptyEntries);
        
        var targetValue = new HeroSaveItem
        {
            Id = int.Parse(dataArray[0]),
            StartTime = DateTimeExtension.UnixTimeToDateTime(long.Parse(dataArray[1]))
        };
        
        return targetValue;
    }
}

[JsonConverter(typeof(HeroSaveItemJsonConverter))]
public class HeroSaveItem
{
    private int id;
    private DateTime startTime;
    
    public int Id
    {
        get { return id; }
        set { id = value; }
    }
    
    public DateTime StartTime
    {
        get { return startTime; }
        set { startTime = value; }
    }
}