using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

public class AreaAccessSaveItemJsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof (AreaAccessSaveItem);
    }
    
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var targetValue = (AreaAccessSaveItem) value;
        
        serializer.TypeNameHandling = TypeNameHandling.None;
        StringBuilder str = new StringBuilder();
        foreach (var point in targetValue.BasePoints)
        {
            str.Append($"{point.X},{point.Y},");
        }
        if(str.Length > 0)
            str.Remove(str.Length - 1, 1);
        serializer.Serialize(writer, str.ToString());
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var targetValue = new AreaAccessSaveItem();

        var data = serializer.Deserialize<string>(reader);
        var dataArray = data.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);

        targetValue.BasePoints = new List<BoardPosition>();
        for (int i = 0; i < dataArray.Length; i+=2)
        {
            var x = Convert.ToInt32(dataArray[i]);
            var y = Convert.ToInt32(dataArray[i + 1]);
            targetValue.BasePoints.Add(new BoardPosition(x, y, 1));
        }
        
        return targetValue;
    }
}

[JsonConverter(typeof(AreaAccessSaveItemJsonConverter))]
public class AreaAccessSaveItem
{
    private List<BoardPosition> basePoints;
    public List<BoardPosition> BasePoints
    {
        get { return basePoints; }
        set { basePoints = value; }
    }
}