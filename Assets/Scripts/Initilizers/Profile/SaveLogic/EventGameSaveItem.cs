using System;
using System.Collections.Generic;
using System.Text;
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
        serializer.Serialize(writer, $"{(int)targetValue.Key};{(int)targetValue.State};{WriteList(targetValue.Steps)}");
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var data = serializer.Deserialize<string>(reader);
        var dataArray = data.Split(new[] {";"}, StringSplitOptions.RemoveEmptyEntries);

        var targetValue = new EventGameSaveItem
        {
            Key = (EventGameType) int.Parse(dataArray[0]),
            State = (EventGameState) int.Parse(dataArray[1]),
            Steps = ReadList(dataArray[2])
        };
        
        return targetValue;
    }

    private string WriteList(List<KeyValuePair<bool, bool>> value)
    {
        var str = new StringBuilder();

        foreach (var pair in value)
        {
            str.Append($"{(pair.Key ? 1 : 0)}");
            str.Append(":");
            str.Append($"{(pair.Value ? 1 : 0)}");
            str.Append(",");
        }
        
        return str.ToString();
    }

    private List<KeyValuePair<bool, bool>> ReadList(string str)
    {
        var result = new List<KeyValuePair<bool, bool>>();
        var data = str.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);

        foreach (var item in data)
        {
            var sData = item.Split(new[] {":"}, StringSplitOptions.RemoveEmptyEntries);
            var key = int.Parse(sData[0]) == 1;
            var value = int.Parse(sData[1]) == 1;
            
            result.Add(new KeyValuePair<bool, bool>(key, value));
        }

        return result;
    }
}

[JsonConverter(typeof(EventGameSaveItemJsonConverter))]
public class EventGameSaveItem
{
    private EventGameType key;
    private EventGameState state;

    private List<KeyValuePair<bool, bool>> steps;
    
    public EventGameType Key
    {
        get => key;
        set => key = value;
    }

    public EventGameState State
    {
        get => state;
        set => state = value;
    }

    public List<KeyValuePair<bool, bool>> Steps
    {
        get => steps;
        set => steps = value;
    }
}