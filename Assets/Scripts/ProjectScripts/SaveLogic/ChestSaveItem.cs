using System;
using Newtonsoft.Json;

public class ChestSaveItemJsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof (ChestSaveItem);
    }
    
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var targetValue = (ChestSaveItem) value;
        
        serializer.TypeNameHandling = TypeNameHandling.None;

        serializer.Serialize(writer, string.Format("{0},{1},{2},{3}",
            targetValue.Id,
            (int) targetValue.State,
            targetValue.StartTime.ConvertToUnixTime(),
            targetValue.Position.ToSaveString()));
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var data = serializer.Deserialize<string>(reader);
        var dataArray = data.Split(new string[] {","}, StringSplitOptions.RemoveEmptyEntries);
        
        var targetValue = new ChestSaveItem
        {
            Id = int.Parse(dataArray[0]),
            State = (ChestState) int.Parse(dataArray[1]),
            StartTime = DateTimeExtension.UnixTimeToDateTime(long.Parse(dataArray[2])),
            Position = new BoardPosition(int.Parse(dataArray[3]), int.Parse(dataArray[4]), int.Parse(dataArray[5]))
        };
        
        return targetValue;
    }
}

[JsonConverter(typeof(ChestSaveItemJsonConverter))]
public class ChestSaveItem
{
    private int id;
    private BoardPosition position;
    private ChestState state;
    private DateTime startTime;
    
    public int Id
    {
        get { return id; }
        set { id = value; }
    }
    
    public BoardPosition Position
    {
        get { return position; }
        set { position = value; }
    }

    public ChestState State
    {
        get { return state; }
        set { state = value; }
    }

    public DateTime StartTime
    {
        get { return startTime; }
        set { startTime = value; }
    }
}