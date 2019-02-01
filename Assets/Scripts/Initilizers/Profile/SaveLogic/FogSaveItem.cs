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
        serializer.Serialize(writer, $"{targetValue.AlreadyPaid},{targetValue.Position.ToSaveString()}");
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var data = serializer.Deserialize<string>(reader);
        var dataArray = data.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
        
        var targetValue = new FogSaveItem
        {
            AlreadyPaid = int.Parse(dataArray[0]),
            Position = new BoardPosition(int.Parse(dataArray[1]), int.Parse(dataArray[2]), int.Parse(dataArray[3]))
        };
        
        return targetValue;
    }
}

[JsonConverter(typeof(FogSaveItemJsonConverter))]
public class FogSaveItem
{
    private BoardPosition position;
    private int alreadyPaid;
    
    public BoardPosition Position
    {
        get { return position; }
        set { position = value; }
    }

    public int AlreadyPaid
    {
        get => alreadyPaid;
        set => alreadyPaid = value;
    }
}