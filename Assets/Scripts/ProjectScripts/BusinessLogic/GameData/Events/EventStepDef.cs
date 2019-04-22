using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class EventSaveItemJsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof (EventStepDef);
    }
    
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var targetValue = (EventStepDef) value;
        
        serializer.TypeNameHandling = TypeNameHandling.None;
        serializer.Serialize(writer, $"{targetValue.Step};{string.Join(",", targetValue.Prices)};{string.Join(",", targetValue.NormalRewards)};{string.Join(",", targetValue.PaidRewards)}");
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var data = serializer.Deserialize<string>(reader);
        var dataArray = data.Split(new[] {";"}, StringSplitOptions.RemoveEmptyEntries);
        
        var targetValue = new EventStepDef
        {
            Step = dataArray[0],
            Prices = ReadList(dataArray[1]),
            NormalRewards = ReadList(dataArray[2]),
            PaidRewards = ReadList(dataArray[3]),
        };
        
        return targetValue;
    }

    private List<CurrencyPair> ReadList(string data)
    {
        var result = new List<CurrencyPair>();
        var dataArray = data.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);

        foreach (var str in dataArray)
        {
            var pair = CurrencyPair.Parse(str);

            if (pair == null) continue;
            
            result.Add(pair);
        }
        
        return result;
    }
}

[JsonConverter(typeof(EventSaveItemJsonConverter))]
public class EventStepDef
{
    public string Step;
    
    public List<CurrencyPair> Prices;
    
    public List<CurrencyPair> NormalRewards;
    public List<CurrencyPair> PaidRewards;
}