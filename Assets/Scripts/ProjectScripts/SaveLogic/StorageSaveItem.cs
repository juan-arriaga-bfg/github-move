using System;
using Newtonsoft.Json;

public class StorageSaveItemJsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof (StorageSaveItem);
    }
    
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var targetValue = (StorageSaveItem) value;
        
        serializer.TypeNameHandling = TypeNameHandling.None;

        serializer.Serialize(writer, string.Format("{0},{1},{2}",
            targetValue.StartTime,
            targetValue.Filling,
            targetValue.Position.ToSaveString()));
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var data = serializer.Deserialize<string>(reader);
        var dataArray = data.Split(new string[] {","}, StringSplitOptions.RemoveEmptyEntries);
        
        var targetValue = new StorageSaveItem
        {
            StartTime = long.Parse(dataArray[0]),
            Filling = int.Parse(dataArray[1]),
            Position = new BoardPosition(int.Parse(dataArray[2]), int.Parse(dataArray[3]), int.Parse(dataArray[4]))
        };
        
        return targetValue;
    }
}

[JsonConverter(typeof(StorageSaveItemJsonConverter))]
public class StorageSaveItem
{
    private BoardPosition position;
    private long startTime;
    private int filling;
    
    public BoardPosition Position
    {
        get { return position; }
        set { position = value; }
    }
    
    public long StartTime
    {
        get { return startTime; }
        set { startTime = value; }
    }

    public int Filling
    {
        get { return filling; }
        set { filling = value; }
    }
}