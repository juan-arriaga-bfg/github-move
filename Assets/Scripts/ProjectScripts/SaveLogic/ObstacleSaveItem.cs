using System;
using Newtonsoft.Json;

public class ObstacleSaveItemJsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof (ObstacleSaveItem);
    }
    
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var targetValue = (ObstacleSaveItem) value;
        
        serializer.TypeNameHandling = TypeNameHandling.None;

        serializer.Serialize(writer, string.Format("{0},{1}",
            targetValue.Step,
            targetValue.Position.ToSaveString()));
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var data = serializer.Deserialize<string>(reader);
        var dataArray = data.Split(new string[] {","}, StringSplitOptions.RemoveEmptyEntries);
        
        var targetValue = new ObstacleSaveItem
        {
            Step = int.Parse(dataArray[0]),
            Position = new BoardPosition(int.Parse(dataArray[1]), int.Parse(dataArray[2]), int.Parse(dataArray[3]))
        };
        
        return targetValue;
    }
}

[JsonConverter(typeof(ObstacleSaveItemJsonConverter))]
public class ObstacleSaveItem
{
    private int step;
    private BoardPosition position;

    public int Step
    {
        get { return step; }
        set { step = value; }
    }
    
    public BoardPosition Position
    {
        get { return position; }
        set { position = value; }
    }
}