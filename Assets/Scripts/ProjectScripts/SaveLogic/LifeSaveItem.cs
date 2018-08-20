using System;
using Newtonsoft.Json;

public class LifeSaveItemJsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof (LifeSaveItem);
    }
    
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var targetValue = (LifeSaveItem) value;
        
        serializer.TypeNameHandling = TypeNameHandling.None;
        serializer.Serialize(writer, $"{targetValue.Step},{targetValue.StartTime},{targetValue.Position.ToSaveString()}");
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var data = serializer.Deserialize<string>(reader);
        var dataArray = data.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
        
        var targetValue = new LifeSaveItem
        {
            Step = int.Parse(dataArray[0]),
            StartTime = long.Parse(dataArray[1]),
            Position = new BoardPosition(int.Parse(dataArray[2]), int.Parse(dataArray[3]), int.Parse(dataArray[4]))
        };
        
        return targetValue;
    }
}

[JsonConverter(typeof(LifeSaveItemJsonConverter))]
public class LifeSaveItem
{
    private int step;
    private long startTime;
    private BoardPosition position;
    
    public int Step
    {
        get { return step; }
        set { step = value; }
    }
    
    public long StartTime
    {
        get { return startTime; }
        set { startTime = value; }
    }
    
    public BoardPosition Position
    {
        get { return position; }
        set { position = value; }
    }
}