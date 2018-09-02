using System;
using Newtonsoft.Json;

public class BuildingSaveItemJsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof (BuildingSaveItem);
    }
    
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var targetValue = (BuildingSaveItem) value;
        
        serializer.TypeNameHandling = TypeNameHandling.None;
        serializer.Serialize(writer, $"{targetValue.Id},{targetValue.Position.ToSaveString()},{(int)targetValue.State},{targetValue.StartTime}");
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var targetValue = new BuildingSaveItem();

        var data = serializer.Deserialize<string>(reader);
        var dataArray = data.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
        var state = (BuildingState) int.Parse(dataArray[4]);
        
        targetValue.Id = int.Parse(dataArray[0]);
        targetValue.Position = new BoardPosition(int.Parse(dataArray[1]), int.Parse(dataArray[2]), int.Parse(dataArray[3]));
        targetValue.State = state == BuildingState.Waiting ? BuildingState.Warning : state;
        targetValue.StartTime = long.Parse(dataArray[5]);
        
        return targetValue;
    }
}

[JsonConverter(typeof(BuildingSaveItemJsonConverter))]
public class BuildingSaveItem
{
    private int id;
    private BoardPosition position;
    private BuildingState state;
    private long startTime;

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

    public BuildingState State
    {
        get { return state; }
        set { state = value; }
    }

    public long StartTime
    {
        get { return startTime; }
        set { startTime = value; }
    }
}