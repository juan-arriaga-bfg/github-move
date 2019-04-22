using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using UnityEngine;

public class AirShipSaveItemJsonConverter : JsonConverter
{
    private static Vector2 StringToVector2(string s) 
    {
        var culture = CultureInfo.InvariantCulture;
        
        string[] temp = s.Split (',');
        return new Vector2 (float.Parse(temp[0], culture), float.Parse(temp[1], culture));
    }
    
    private static string Vector2String(Vector2 v)
    {
        var culture = CultureInfo.InvariantCulture;
        return $"{v.x.ToString(culture)},{v.y.ToString(culture)}";
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof (AirShipSaveItem);
    }
    
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var targetValue = (AirShipSaveItem) value;
        
        serializer.TypeNameHandling = TypeNameHandling.None;
        serializer.Serialize(writer, $"{Vector2String(targetValue.Position)};{targetValue.Payload.ToSaveString()}");
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var data = serializer.Deserialize<string>(reader);
        var dataArray = data.Split(new[] {";"}, StringSplitOptions.RemoveEmptyEntries);
        var payloadArray = dataArray[1].Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
        var targetValue = new AirShipSaveItem
        {
            Position = StringToVector2(dataArray[0]),
            Payload = DictionaryStringConverter.FromDataArray(payloadArray, int.Parse, int.Parse, 0)
        };
        
        return targetValue;
    }
}

[JsonConverter(typeof(AirShipSaveItemJsonConverter))]
public class AirShipSaveItem
{
    public Vector2 Position;
    public Dictionary<int, int> Payload;
}