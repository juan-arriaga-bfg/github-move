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
        serializer.Serialize(writer, $"{targetValue.Step},{targetValue.Position.ToSaveString()},{(targetValue.IsStartTimer ? 1 : 0)},{targetValue.StartTimeTimer},{(targetValue.IsStartCooldown ? 1 : 0)},{targetValue.StartTimeCooldown}");
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var data = serializer.Deserialize<string>(reader);
        var dataArray = data.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
        
        var targetValue = new LifeSaveItem
        {
            Step = int.Parse(dataArray[0]),
            Position = new BoardPosition(int.Parse(dataArray[1]), int.Parse(dataArray[2]), int.Parse(dataArray[3])),
            IsStartTimer = int.Parse(dataArray[4]) == 1,
            StartTimeTimer = long.Parse(dataArray[5]),
            IsStartCooldown = int.Parse(dataArray[6]) == 1,
            StartTimeCooldown = long.Parse(dataArray[7])
        };
        
        return targetValue;
    }
}

public class LoopSaveItemJsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof (LoopSaveItem);
    }
    
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var targetValue = (LoopSaveItem) value;
        
        serializer.TypeNameHandling = TypeNameHandling.None;
        serializer.Serialize(writer, $"{targetValue.Uid},{targetValue.Value}");
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var data = serializer.Deserialize<string>(reader);
        var dataArray = data.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
        
        var targetValue = new LoopSaveItem
        {
            Uid = int.Parse(dataArray[0]),
            Value = int.Parse(dataArray[1]),
        };
        
        return targetValue;
    }
}

[JsonConverter(typeof(LifeSaveItemJsonConverter))]
public class LifeSaveItem
{
    private BoardPosition position;
    private int step;
    
    private bool isStartTimer;
    private long startTimeTimer;
    
    private bool isStartCooldown;
    private long startTimeCooldown;
    
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
    
    public bool IsStartTimer
    {
        get { return isStartTimer; }
        set { isStartTimer = value; }
    }
    
    public long StartTimeTimer
    {
        get { return startTimeTimer; }
        set { startTimeTimer = value; }
    }
    
    public bool IsStartCooldown
    {
        get { return isStartCooldown; }
        set { isStartCooldown = value; }
    }
    
    public long StartTimeCooldown
    {
        get { return startTimeCooldown; }
        set { startTimeCooldown = value; }
    }
}

[JsonConverter(typeof(LoopSaveItemJsonConverter))]
public class LoopSaveItem
{
    private int uid;
    private int value;

    public int Uid
    {
        get => uid;
        set => uid = value;
    }

    public int Value
    {
        get => value;
        set => this.value = value;
    }
}