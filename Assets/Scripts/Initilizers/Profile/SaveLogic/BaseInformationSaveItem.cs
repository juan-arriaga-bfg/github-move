using System;
using Newtonsoft.Json;

public class BaseInformationSaveItemJsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof (MarketSaveItem);
    }
    
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var targetValue = (BaseInformationSaveItem) value;
        
        serializer.TypeNameHandling = TypeNameHandling.None;
        serializer.Serialize(writer, $"{targetValue.ProfileCreationDate.ToShortDateString()},{targetValue.IsPayer}");
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var data = serializer.Deserialize<string>(reader);
        var dataArray = data.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
        
        var targetValue = new BaseInformationSaveItem()
        {
            ProfileCreationDate = DateTime.Parse(dataArray[0]),
            IsPayer = bool.Parse(dataArray[1])
        };
        
        return targetValue;
    }
}

[JsonConverter(typeof(BaseInformationSaveItemJsonConverter))]
public class BaseInformationSaveItem
{
    private DateTime profileCreationDate = DateTime.UtcNow;
    private bool isPayer = false;

    public DateTime ProfileCreationDate
    {
        get { return profileCreationDate; }
        set { profileCreationDate = value; }
    }

    public bool IsPayer
    {
        get { return isPayer; }
        set { isPayer = value; }
    }
}