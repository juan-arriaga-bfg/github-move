using System;
using Newtonsoft.Json;

public class EventGameSaveItemJsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof (EventGameSaveItem);
    }
    
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var targetValue = (EventGameSaveItem) value;
        
        serializer.TypeNameHandling = TypeNameHandling.None;
        serializer.Serialize(writer, $"{(int)targetValue.Key},{targetValue.Step},{(targetValue.IsNormalClaimed ? 1 : 0)},{(targetValue.IsPremiumClaimed ? 1 : 0)}");
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var data = serializer.Deserialize<string>(reader);
        var dataArray = data.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);

        var targetValue = new EventGameSaveItem
        {
            Key = (EventGameType) int.Parse(dataArray[0]),
            Step = dataArray[1],
            IsNormalClaimed = int.Parse(dataArray[2]) == 1,
            IsPremiumClaimed = int.Parse(dataArray[3]) == 1
        };
        
        return targetValue;
    }
}

[JsonConverter(typeof(EventGameSaveItemJsonConverter))]
public class EventGameSaveItem
{
    private EventGameType key;
    private string step;

    private bool isNormalClaimed;
    private bool isPremiumClaimed;

    public EventGameType Key
    {
        get { return key; }
        set { key = value; }
    }
    
    public string Step
    {
        get { return step; }
        set { step = value; }
    }
    
    public bool IsNormalClaimed
    {
        get { return isNormalClaimed; }
        set { isNormalClaimed = value; }
    }
    
    public bool IsPremiumClaimed
    {
        get { return isPremiumClaimed; }
        set { isPremiumClaimed = value; }
    }
}