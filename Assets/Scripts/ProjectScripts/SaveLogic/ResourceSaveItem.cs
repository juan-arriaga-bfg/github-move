using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

public class ResourceSaveItemJsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof (ResourceSaveItem);
    }
    
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var targetValue = (ResourceSaveItem) value;
        var str = new StringBuilder();

        foreach (var resource in targetValue.Resources)
        {
            var save = resource.ToSaveString();
            
            if(string.IsNullOrEmpty(save)) continue;
            
            str.Append(save);
            str.Append(";");
        }
        
        serializer.TypeNameHandling = TypeNameHandling.None;

        serializer.Serialize(writer, string.Format("{0}:{1}", targetValue.Position.ToSaveString(), str));
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var targetValue = new ResourceSaveItem{Resources = new List<CurrencyPair>()};

        var data = serializer.Deserialize<string>(reader);
        var dataArray = data.Split(new string[] {":"}, StringSplitOptions.RemoveEmptyEntries);
        var resArray = dataArray[1].Split(new string[] {";"}, StringSplitOptions.RemoveEmptyEntries);
        
        targetValue.Position = BoardPosition.Parse(dataArray[0]);
        
        foreach (var str in resArray)
        {
            var pair = CurrencyPair.Parse(str);
            
            if(pair == null) continue;
            
            targetValue.Resources.Add(pair);
        }
        
        return targetValue;
    }
}

[JsonConverter(typeof(ResourceSaveItemJsonConverter))]
public class ResourceSaveItem
{
    private BoardPosition position;
    private List<CurrencyPair> resources;

    public BoardPosition Position
    {
        get { return position; }
        set { position = value; }
    }

    public List<CurrencyPair> Resources
    {
        get { return resources; }
        set { resources = value; }
    }
}